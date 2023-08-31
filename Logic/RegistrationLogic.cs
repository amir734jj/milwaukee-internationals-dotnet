using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using DAL.Interfaces;
using Flurl;
using Logic.Interfaces;
using Models.Constants;
using Models.Entities;
using Models.Enums;
using Net.Codecrete.QrCodeGenerator;
using Svg;

namespace Logic;

public class RegistrationLogic : IRegistrationLogic
{
    private readonly IStudentLogic _studentLogic;
    private readonly IDriverLogic _driverLogic;
    private readonly IHostLogic _hostLogic;
    private readonly IEventLogic _eventLogic;
    private readonly ILocationLogic _locationLogic;
    private readonly IEmailServiceApi _emailServiceApiApi;
    private readonly IApiEventService _apiEventService;
    private readonly ISmsService _smsService;
    private readonly GlobalConfigs _globalConfigs;

    /// <summary>
    /// Constructor dependency injection
    /// </summary>
    /// <param name="studentLogic"></param>
    /// <param name="driverLogic"></param>
    /// <param name="hostLogic"></param>
    /// <param name="eventLogic"></param>
    /// <param name="locationLogic"></param>
    /// <param name="emailServiceApiApi"></param>
    /// <param name="apiEventService"></param>
    /// <param name="smsService"></param>
    /// <param name="globalConfigs"></param>
    public RegistrationLogic(
        IStudentLogic studentLogic,
        IDriverLogic driverLogic,
        IHostLogic hostLogic,
        IEventLogic eventLogic,
        ILocationLogic locationLogic,
        IEmailServiceApi emailServiceApiApi,
        IApiEventService apiEventService,
        ISmsService smsService,
        GlobalConfigs globalConfigs)
    {
        _studentLogic = studentLogic;
        _driverLogic = driverLogic;
        _hostLogic = hostLogic;
        _eventLogic = eventLogic;
        _locationLogic = locationLogic;
        _emailServiceApiApi = emailServiceApiApi;
        _apiEventService = apiEventService;
        _smsService = smsService;
        _globalConfigs = globalConfigs;
    }

    public async Task SendStudentEmail(Student student)
    {
        const string rootUrl = ApiConstants.SiteUrl;
        var checkInPath = Url.Combine(rootUrl, "App", "CheckIn", "Student", student.GenerateHash());

        var qr = QrCode.EncodeText(checkInPath, QrCode.Ecc.High);
        var svg = qr.ToSvgString(4);

        var doc = new XmlDocument();
        doc.LoadXml(svg);

        var svgDocument = SvgDocument.Open(doc);
        using var smallBitmap = svgDocument.Draw();

        using var bitmap = svgDocument.Draw(400, 400);
        var ms = new MemoryStream();
#pragma warning disable CA1416
        bitmap.Save(ms, ImageFormat.Png);
#pragma warning restore CA1416

        var sigBase64 = Convert.ToBase64String(ms.ToArray());
        var qrUri = $"data:image/png;base64,{sigBase64}";

        await _emailServiceApiApi.SendEmailAsync(student.Email, "Tour of Milwaukee Registration Confirmation",
            $@"
                    <p>Name: {student.Fullname}</p>
                    <p>University: {student.University}</p>
                    <p>Major: {student.Major}</p>
                    <p>Phone: {student.Phone}</p>
                    <hr>
                    <p>See you at the Tour of Milwaukee</p>
                    <p> Note that this is not a bus tour; it's a personal tour with 2-4 people in each vehicle. The tour concludes with a dinner at an American home. </p>
                    <br>
                    <p> {_globalConfigs.TourDate.Year} Tour of Milwaukee</p>
                    <p> Date: {_globalConfigs.TourDate:dddd, MMMM d, yyyy}</p>
                    <p> Time: 12:00 noon</p>
                    <p> Address: {_globalConfigs.TourAddress} </p>
                    <p> Location: {_globalConfigs.TourLocation} </p>
                    <p> Thank you for registering for this event. Please share this with other new international friends.</p>
                    <p> If you need any sort of help (furniture, etc.), please contact Asher Imtiaz (414-499-5360).</p>
                    <br>
                    {(_globalConfigs.QrInStudentEmail ? @$"<br>
                    <p>Please save your QR code after your online registration. You can use the QR code when you check-in on the day of the Tour of Milwaukee.</p>
                    <img src=""{qrUri}"" alt=""QR code"" />" : "")}
                ");
    }

    public async Task SendDriverEmail(Driver driver)
    {
        switch (driver.Role)
        {
            case RolesEnum.Driver:
                await _emailServiceApiApi.SendEmailAsync(driver.Email, "Tour of Milwaukee: Driver registration",
                    $@"
                    <p> Name: {driver.Fullname}</p>
                    <p> Role: {driver.Role}</p>
                    <p> Phone: {driver.Phone}</p>
                    <p> Capacity: {driver.Capacity}</p>
                    <p> Display Id: {driver.DisplayId}</p>
                    <p> Require Navigator: {(driver.RequireNavigator ? "Yes, navigator will be assigned to you" : $"No, my navigator is: {driver.Navigator}")}</p>
                    <br>
                    <p> {_globalConfigs.TourDate.Year} Tour of Milwaukee</p>
                    <p> Date: {_globalConfigs.TourDate:dddd, MMMM d, yyyy}</p>
                    <p> Time: 12:00 noon (Brief orientation only for drivers and navigators) </p>
                    <p> Address: {_globalConfigs.TourAddress} </p>
                    <p> Location: {_globalConfigs.TourLocation} </p>
                    <br>
                    <p> Thank you for helping with the tour this year.</p>
                    <p> For questions, comments and feedback, please contact Asher Imtiaz (414-499-5360) or Marie Wilke (414-852-5132).</p>
                    <br>
                    <p> Blessings, </p>
                ");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task SendHostEmail(Host host)
    {
        await _emailServiceApiApi.SendEmailAsync(host.Email, "Tour of Milwaukee: Host registration", $@"
                    <p>Name: {host.Fullname}</p>
                    <p>Address: {host.Address}</p>
                    <hr>
                    <p>Thank you for helping with hosting and welcoming Internationals to Milwaukee.</p>
                    <br>
                    <p> {_globalConfigs.TourDate.Year} Tour of Milwaukee</p>
                    <p> Date: {_globalConfigs.TourDate:dddd, MMMM d, yyyy}</p>
                    <p> Time: 5:30 pm</p>
                    <p> We will send you more details once we have them.</p>
                    <p> For questions, any change in plans, please contact Asher Imtiaz (414-499-5360).</p>
                    <p> Blessings,</p>
                ");
    }

    public async Task<bool> IsRegisterStudentOpen()
    {
        var year = DateTime.Now.Year;
        var count = await _studentLogic.Count(x => x.Year == year);

        return count <= _globalConfigs.MaxLimitStudentSeats;
    }
    
    
    public async Task RegisterDriver(Driver driver)
    {
        driver = await _driverLogic.Save(driver);

        // If save was successful
        if (driver != null)
        {
            await _apiEventService.RecordEvent($"Driver {driver.Fullname} registered");

            await SendDriverEmail(driver);
        }
    }

    public async Task RegisterStudent(Student student)
    {
        student = await _studentLogic.Save(student);

        // If save was successful
        if (student != null)
        {
            await _apiEventService.RecordEvent($"Student {student.Fullname} registered");
            
            await SendStudentEmail(student);
        }
    }

    public async Task RegisterHost(Host host)
    {
        host = await _hostLogic.Save(host);

        // If save was successful
        if (host != null)
        {
            await _apiEventService.RecordEvent($"Host {host.Fullname} registered");

            await SendHostEmail(host);
        }
    }

    public async Task RegisterEvent(Event @event)
    {
        await _eventLogic.Save(@event);
    }

    public async Task RegisterLocation(Location location)
    {
        await _locationLogic.Save(location);
    }

    public async Task SendDriverSms(Driver driver)
    {
        var text = $"Thank you for being a driver,\n" +
                   $"you have {driver.Students.Count} students to drive\n" +
                   $"students: {string.Join(", ", driver.Students.Select(x => x.Fullname))}\n" +
                   $"milwaukeeinternationals.com/places";
        await _smsService.SendMessage(driver.Phone, text);
    }
}