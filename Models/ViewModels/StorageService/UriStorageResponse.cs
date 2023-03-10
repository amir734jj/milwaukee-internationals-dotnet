using System;
using System.Collections.Generic;
using System.Net;

namespace Models.ViewModels.StorageService;

public class UriStorageResponse : SimpleStorageResponse
{
    public Uri Uri { get; }

    public IReadOnlyDictionary<string, string> MetaData { get; }
        
    public string ContentType { get; }
        
    public string Name { get; }
        
    public UriStorageResponse(HttpStatusCode status, string message) : base(status, message) { }

    public UriStorageResponse(HttpStatusCode status, string message, Uri uri, IReadOnlyDictionary<string, string> metaData, string contentType, string name) : base(status, message)
    {
        Uri = uri;
        MetaData = metaData;
        ContentType = contentType;
        Name = name;
    }
}