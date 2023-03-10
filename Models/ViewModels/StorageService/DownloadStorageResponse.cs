using System.Collections.Generic;
using System.Net;

namespace Models.ViewModels.StorageService;

public class DownloadStorageResponse : SimpleStorageResponse
{
    public byte[] Data { get; }
        
    public IDictionary<string, string> MetaData { get; }
        
    public string ContentType { get; }
        
    public string Name { get; }

    public DownloadStorageResponse(HttpStatusCode statusCode, string message) : base(statusCode, message)
    {
    }

    public DownloadStorageResponse(HttpStatusCode status, string message, byte[] data, IDictionary<string, string> metaData, string contentType, string name) : base(status, message)
    {
        Data = data;
        MetaData = metaData;
        ContentType = contentType;
        Name = name;
    }
}