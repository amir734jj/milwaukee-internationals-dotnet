using System.Net;

namespace Models.ViewModels.StorageService;

public class SimpleStorageResponse
{
    public HttpStatusCode Status { get; }
        
    public string Message { get; }

    public SimpleStorageResponse(HttpStatusCode status, string message)
    {
        Status = status;
        Message = message;
    }
}