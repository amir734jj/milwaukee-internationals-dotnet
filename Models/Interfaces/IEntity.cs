using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Models.Interfaces;

public interface IEntity
{
    [Key]
    public int Id { get; set; }
        
    private static readonly JsonSerializerSettings JsonSerializerSettings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        
    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this, JsonSerializerSettings);
    }
}