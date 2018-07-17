using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IEmailServiceApi
    {
        Task SendEmailAsync(string emailAddress, string emailSubject, string emailText);
    }
}