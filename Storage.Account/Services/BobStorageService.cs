using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Storage.Account.Demo.Services
{
    public class BobStorageService : IBobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private string containerName;


        public BobStorageService(IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            _configuration = configuration;
            _blobServiceClient = blobServiceClient;
            containerName = _configuration["BlobContainers:ContainerAGS"];
        }



        public async Task<string> UploadBlob(IFormFile file, string fileName,
            string? originalBlobName = null)
        {
            try
            {
                var blobName = $"{fileName}{Path.GetExtension(file.FileName)}";

                var container =  _blobServiceClient.GetBlobContainerClient(containerName);

                //remove o blob antigo
                if (!string.IsNullOrEmpty(originalBlobName))
                    await Removelob(originalBlobName);

                using var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var blob = container.GetBlobClient(blobName);

                //faz o upload do blob ou substitui se ja existir
                await blob.UploadAsync(content: memoryStream, overwrite: true);

                return blobName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetBlobUrl(string fileName)
        {

            try
            {
                var container = _blobServiceClient.GetBlobContainerClient(containerName);

                var blob = container.GetBlobClient(fileName);

                //permissoes de leitura
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blob.BlobContainerName,
                    BlobName = blob.Name,
                    Resource = "b", // especifica que o recurso é um blob
                    Protocol = SasProtocol.Https,
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2) //tempo de expiracao
                };

                sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

                //retorna a url com a permissao de leitura
                return blob.GenerateSasUri(sasBuilder).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Removelob(string fileName)
        {
            try
            {
                var container = _blobServiceClient.GetBlobContainerClient(containerName);

                var blob = container.GetBlobClient(fileName);

                //deleta o blob e os snapshots
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<BlobContainerClient> GetBlobContainerClient()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("StorageConnectionStrings");
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();
                return containerClient;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
