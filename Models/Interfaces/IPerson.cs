using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Models.Interfaces;

public interface IPerson : IEntity
{
    string Fullname { get; set; }
    
    [Obsolete("Obsolete")]
    public static string ComputeHash<T>(T self)
    {
        using var sha1 = new SHA1Managed();
        
        byte[] inputBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(self));
        byte[] hashBytes = sha1.ComputeHash(inputBytes);
            
        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return hashString;
    }
}