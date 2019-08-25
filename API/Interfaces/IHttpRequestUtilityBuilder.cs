using API.Utilities;
using Microsoft.AspNetCore.Http;

namespace API.Interfaces
{
    public interface IHttpRequestUtilityBuilder
    {
        HttpRequestUtility For(HttpContext ctx);
    }
}