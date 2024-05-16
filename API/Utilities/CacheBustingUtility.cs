using System.Web;

namespace API.Utilities;

public class CacheBustingUtility
{
    private readonly string _cacheBustingKey = HttpUtility.UrlEncode(AssemblyInfo.AssemblyVersion);

    public string CacheBustingKey()
    {
        return _cacheBustingKey;
    }
}