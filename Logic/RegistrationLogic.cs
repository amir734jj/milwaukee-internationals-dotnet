using System;
using System.Drawing.Imaging;
using System.IO;
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

namespace Logic
{
    public class RegistrationLogic : IRegistrationLogic
    {
        private readonly IStudentLogic _studentLogic;
        private readonly IDriverLogic _driverLogic;
        private readonly IHostLogic _hostLogic;
        private readonly IEventLogic _eventLogic;
        private readonly IEmailServiceApi _emailServiceApiApi;
        private readonly IApiEventService _apiEventService;
        private readonly GlobalConfigs _globalConfigs;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="studentLogic"></param>
        /// <param name="driverLogic"></param>
        /// <param name="hostLogic"></param>
        /// <param name="eventLogic"></param>
        /// <param name="emailServiceApiApi"></param>
        /// <param name="apiEventService"></param>
        /// <param name="globalConfigs"></param>
        public RegistrationLogic(IStudentLogic studentLogic, IDriverLogic driverLogic, IHostLogic hostLogic,
            IEventLogic eventLogic, IEmailServiceApi emailServiceApiApi, IApiEventService apiEventService, GlobalConfigs globalConfigs)
        {
            _studentLogic = studentLogic;
            _driverLogic = driverLogic;
            _hostLogic = hostLogic;
            _eventLogic = eventLogic;
            _emailServiceApiApi = emailServiceApiApi;
            _apiEventService = apiEventService;
            _globalConfigs = globalConfigs;
        }

        /// <summary>
        /// Register driver
        /// </summary>
        /// <param name="driver"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public async Task RegisterDriver(Driver driver)
        {
            driver = await _driverLogic.Save(driver);

            // If save was successful
            if (driver != null)
            {
                await _apiEventService.RecordEvent($"Driver {driver.Fullname} registered");

                switch (driver.Role)
                {
                    case RolesEnum.Driver:
                        await _emailServiceApiApi.SendEmailAsync(driver.Email, "Tour of Milwaukee: Driver registration",
                            $@"
                    <p> This is an automatically generated email. </p>
                    <p> ----------------------------------------- </p>
                    <p> Name: {driver.Fullname}</p>
                    <p> Role: {driver.Role}</p>
                    <p> Phone: {driver.Phone}</p>
                    <p> Capacity: {driver.Capacity}</p>
                    <p> Display Id: {driver.DisplayId}</p>
                    <p> Require Navigator: {(driver.RequireNavigator ? "Yes, navigator will be assigned to you" : $"No, my navigator is: {driver.Navigator}")}</p>
                    <br>
                    <p> {DateTime.Now.Year} Tour of Milwaukee</p>
                    <p> Date: September 3, {DateTime.Now.Year}</p>
                    <p> Time: 12:30 pm (Brief orientation only for drivers and navigators) </p>
                    <p> Address: 3202 N Maryland Ave, Milwaukee, WI 53211 </p>
                    <p> Location: Lubar Hall (Business Building, UWM) </p>
                    <br>
                    <p> Thank you for helping with the tour this year.</p>
                    <p> For questions, comments and feedback, please contact Asher Imtiaz (414-499-5360) or Marie Wilke (414-852-5132).</p>
                    <br>
                    <p> Blessings, </p>
                ");
                        break;
                    case RolesEnum.Navigator:
                        await _emailServiceApiApi.SendEmailAsync(driver.Email, "Tour of Milwaukee: Driver registration",
                            $@"
                    <p> This is an automatically generated email. </p>
                    <p> ----------------------------------------- </p>
                    <p> Name: {driver.Fullname}</p>
                    <p> Role: {driver.Role}</p>
                    <p> Phone: {driver.Phone}</p>
                    <p> Display Id: {driver.DisplayId}</p>
                    <br>
                    <p> {DateTime.Now.Year} Tour of Milwaukee</p>
                    <p> Date: September 3, {DateTime.Now.Year}</p>
                    <p> Time: 12:30 pm (Brief orientation only for drivers and navigators) </p>
                    <p> Address: 3202 N Maryland Ave, Milwaukee, WI 53211 </p>
                    <p> Location: Lubar Hall (Business Building, UWM) </p>
                    <br>
                    <p> Thank you for helping with the tour this year.</p>
                    <p> For questions, comments and feedback, please contact Asher Imtiaz (414-499-5360).</p>
                    <br>
                    <p> Blessings, </p>
                ");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Registers student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public async Task RegisterStudent(Student student)
        {
            student = await _studentLogic.Save(student);
            
            // If save was successful
            if (student != null)
            {
                await _apiEventService.RecordEvent($"Student {student.Fullname} registered");

                const string rootUrl = ApiConstants.SiteUrl;
                var checkInPath = Url.Combine(rootUrl, "App", "CheckIn", "Student", student.GetHashCode().ToString());

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
                    <p>Email: {student.Email}</p>
                    <p>University: {student.University}</p>
                    <p>Major: {student.Major}</p>
                    <p>Phone: {student.Phone}</p>
                    <hr>
                    <p>See you at the Tour of Milwaukee</p>
                    <br>
                    <p> {DateTime.Now.Year} Tour of Milwaukee Registration</p>
                    <p> Date: September 3, {DateTime.Now.Year}</p>
                    <p> Time: 12:00 noon</p>
                    <p> Address: 3202 N Maryland Ave, Milwaukee, WI 53211 </p>
                    <p> Location: Lubar Hall (Business Building, UWM) </p>
                    <p> Thank you for registering for this event. Please share this with other new international friends.</p>
                    <p> If you need any sort of help (furniture, etc.), please contact Asher Imtiaz (414-499-5360).</p>
                    <br>
                    {(_globalConfigs.QrInStudentEmail ? @$"<br>
                    <p>Please save your QR code after your online registration. You can use the QR code when you check-in on the day of the Tour of Milwaukee.</p>
                    <img src=""{qrUri}"" alt=""QR code"" />" : "")}
                ");
            }
        }

        /// <summary>
        /// Registers host
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public async Task RegisterHost(Host host)
        {
            host = await _hostLogic.Save(host);
            
            // If save was successful
            if (host != null)
            {
                await _apiEventService.RecordEvent($"Host {host.Fullname} registered");

                await _emailServiceApiApi.SendEmailAsync(host.Email, "Tour of Milwaukee: Host registration", $@"
                    <p> This is an automatically generated email. </p>
                    <p> ----------------------------------------- </p>
                    <p>Name: {host.Fullname}</p>
                    <p>Email: {host.Email}</p>
                    <p>Address: {host.Address}</p>
                    <hr>
                    <p>Thank you for helping with hosting and welcoming Internationals to Milwaukee.</p>
                    <br>
                    <p> Date: September 3, {DateTime.Now.Year}</p>
                    <p> Time: 5:00 pm</p>
                    <p> We will send you more details once we have them.</p>
                    <p> For questions, any change in plans, please contact Asher Imtiaz (414-499-5360).</p>
                    <p> Blessings,</p>
                ");
            }
        }

        public async Task RegisterEvent(Event @event)
        {
            await _eventLogic.Save(@event);
        }
    }
}
