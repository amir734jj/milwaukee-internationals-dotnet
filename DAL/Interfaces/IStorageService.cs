using System.Collections.Generic;
using System.Threading.Tasks;
using Models.ViewModels.StorageService;

namespace DAL.Interfaces
{
    public interface IStorageService
    {
        Task<SimpleStorageResponse> Upload(string fileKey,
            byte[] data,
            IDictionary<string, string> metadata);

        Task<DownloadStorageResponse> Download(string keyName);

        Task<List<string>> List();
    }
}