using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DAL.Configs;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Models.ViewModels.StorageService;

namespace DAL.ServiceApi;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _client;
    private readonly ILogger<S3StorageService> _logger;
    private readonly S3ServiceConfig _s3ServiceConfig;

    /// <summary>
    /// Constructor that takes a S3Client and a prefix for all paths
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="client"></param>
    /// <param name="s3ServiceConfig"></param>
    public S3StorageService(ILogger<S3StorageService> logger, IAmazonS3 client, S3ServiceConfig s3ServiceConfig)
    {
        _logger = logger;
        _client = client;
        _s3ServiceConfig = s3ServiceConfig;
    }

    /// <summary>
    /// Upload a file to an S3, here four files are uploaded in four different ways
    /// </summary>
    /// <param name="fileKey"></param>
    /// <param name="data"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public async Task<SimpleStorageResponse> Upload(string fileKey, byte[] data, IDictionary<string, string> metadata)
    {
        try
        {
            if (await _client.DoesS3BucketExistAsync(_s3ServiceConfig.BucketName))
            {
                var fileTransferUtility = new TransferUtility(_client);

                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    Key = $"{_s3ServiceConfig.Prefix}/{fileKey}",
                    InputStream = new MemoryStream(data),
                    BucketName = _s3ServiceConfig.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                foreach (var (key, value) in metadata)
                {
                    fileTransferUtilityRequest.Metadata.Add(key, value);
                }

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                return new SimpleStorageResponse(HttpStatusCode.OK, "Successfully uploaded to S3");
            }

            // Bucket not found
            throw new Exception($"Bucket: {_s3ServiceConfig.BucketName} does not exist");
        }
        // Catch specific amazon errors
        catch (AmazonS3Exception e)
        {
            _logger.LogError(e, "Failed to upload {}", fileKey);
                
            return new SimpleStorageResponse(e.StatusCode, e.Message);
        }
        // Catch other errors
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to upload {}", fileKey);
                
            return new SimpleStorageResponse(HttpStatusCode.BadRequest, e.Message);
        }
    }

    /// <summary>
    /// Download S3 object
    /// </summary>
    /// <param name="keyName"></param>
    /// <returns></returns>
    public async Task<DownloadStorageResponse> Download(string keyName)
    {
        try
        {
            // Build the request with the bucket name and the keyName (name of the file)
            var request = new GetObjectRequest
            {
                BucketName = _s3ServiceConfig.BucketName,
                Key = $"{_s3ServiceConfig.Prefix}/{keyName}"
            };

            using var response = await _client.GetObjectAsync(request);
            await using var responseStream = response.ResponseStream;
            await using var memoryStream = new MemoryStream();
            var title = response.Metadata["x-amz-meta-title"];
            var contentType = response.Headers["Content-Type"];
            var metadata = response.Metadata.Keys.ToDictionary(x => x, x => response.Metadata[x]);

            // Copy stream to another stream
            await responseStream.CopyToAsync(memoryStream);

            return new DownloadStorageResponse(HttpStatusCode.OK, "Successfully downloaded S3 object", memoryStream.ToArray(), metadata, contentType,
                title);
        }
        // Catch specific amazon errors
        catch (AmazonS3Exception e)
        {
            _logger.LogError(e, "Failed to download {}", keyName);
                
            return new DownloadStorageResponse(e.StatusCode, e.Message);
        }
        // Catch other errors
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to download {}", keyName);
                
            return new DownloadStorageResponse(HttpStatusCode.BadRequest, e.Message);
        }
    }

    public async Task<List<string>> List()
    {
        var request = new ListObjectsV2Request
        {
            BucketName = _s3ServiceConfig.BucketName,
            Prefix = _s3ServiceConfig.Prefix
        };

        var result = await _client.ListObjectsV2Async(request);

        return result.S3Objects?.Select(x => x.Key).ToList();
    }
}