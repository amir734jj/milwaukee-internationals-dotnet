using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using DAL.Interfaces;
using Models.ViewModels.StorageService;

namespace DAL.ServiceApi;

public class AzureBlobService : IStorageService
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureBlobService(BlobContainerClient blobContainerClient)
    {
        _blobContainerClient = blobContainerClient;
    }
    
    public async Task<SimpleStorageResponse> Upload(string fileKey, byte[] data, IDictionary<string, string> metadata)
    {
        await _blobContainerClient.UploadBlobAsync(fileKey, new MemoryStream(data));

        await _blobContainerClient.GetBlobClient(fileKey).SetMetadataAsync(metadata);

        return new SimpleStorageResponse(HttpStatusCode.OK, $"Successfully uploaded blob {fileKey}");
    }

    public async Task<DownloadStorageResponse> Download(string keyName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(keyName);
        var response = await blobClient.DownloadAsync();

        var properties = await blobClient.GetPropertiesAsync();
        
        var stream = new MemoryStream();
        await response.Value.Content.CopyToAsync(stream);

        return new DownloadStorageResponse(HttpStatusCode.OK, $"Successfully downloaded blob {keyName}",
            stream.ToArray(), properties.Value.Metadata, response.Value.ContentType, keyName);
    }

    public async Task<List<string>> List()
    {
        var blobItems = _blobContainerClient.GetBlobs();

        return blobItems.Select(x => x.Name).ToList();
    }
}