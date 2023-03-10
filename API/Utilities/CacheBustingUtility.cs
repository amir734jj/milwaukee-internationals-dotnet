using System.Web;
using Models.Constants;

namespace API.Utilities;

public class CacheBustingUtility
{
    private readonly string _cacheBustingKey;

    public CacheBustingUtility(GlobalConfigs globalConfigs)
    {
        _cacheBustingKey = HttpUtility.UrlEncode(AssemblyInfo.AssemblyVersion + globalConfigs.LastModified.Ticks);
    }
        
    public string CacheBustingKey()
    {
        return _cacheBustingKey;
    }
}