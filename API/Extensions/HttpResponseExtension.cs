using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpResponseExtension
    {
        public static bool IsFailure(this HttpResponse response)
        {
            return response.StatusCode is not (>= 200 and <= 299); 
        }
    }
}