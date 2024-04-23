
namespace Storage.Account.Demo.Services
{
    public interface IBobStorageService
    {
        Task<string> GetBlobUrl(string fileName);
        Task Removelob(string fileName);
        Task<string> UploadBlob(IFormFile file, string fileName, string? originalBlobName = null );
    }
}