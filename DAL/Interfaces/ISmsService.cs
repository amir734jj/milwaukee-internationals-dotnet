using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface ISmsService
{
    Task SendMessage(string phoneNumber, string message);
    
    
    Task SendMessage(IEnumerable<string> phoneNumbers, string message);
}