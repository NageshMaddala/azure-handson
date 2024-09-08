
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using SampleBlobProject.Models;

namespace SampleBlobProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;

        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(name);

            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllBlobs(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            var blobs = blobContainerClient.GetBlobsAsync();

            var blobString = new List<string>();

            await foreach (var item in blobs)
            {
                blobString.Add(item.Name);
            }

            return blobString;
        }

        public async Task<List<Blob>> GetAllBlobsWithUri(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            var blobs = blobContainerClient.GetBlobsAsync();

            var blobList = new List<Blob>();
            string sasContainerSignature = "";

            // sas builder at the container level

            if (blobContainerClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobContainerClient.Name,
                    Resource = "c"
                };

                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);

                sasBuilder.SetPermissions(BlobAccountSasPermissions.Read | BlobAccountSasPermissions.Add);
                // this will generate the uri with sas token
                // image gets displayed even if the container is private
                sasContainerSignature = blobContainerClient.GenerateSasUri(sasBuilder).AbsoluteUri.Split('?')[1].ToString();
            }


            await foreach (var item in blobs)
            {
                var blobClient = blobContainerClient.GetBlobClient(item.Name);
                Blob blobInd = new Blob()
                {
                    Uri = blobClient.Uri.AbsoluteUri + "?" + sasContainerSignature
				};

                //if(blobClient.CanGenerateSasUri)
                //{
                //    BlobSasBuilder sasBuilder = new BlobSasBuilder()
                //    {
                //        BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                //        BlobName = blobClient.Name,
                //        Resource = "b"
                //    };

                //    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);

                //    sasBuilder.SetPermissions(BlobAccountSasPermissions.Read | BlobAccountSasPermissions.Add);
                //    // this will generate the uri with sas token
                //    // image gets displayed even if the container is private
                //    blobInd.Uri = blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
                //}

                BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                if (blobProperties.Metadata.ContainsKey("title"))
                    blobInd.Title = blobProperties.Metadata["title"];

                if (blobProperties.Metadata.ContainsKey("comment"))
                    blobInd.Comment = blobProperties.Metadata["comment"];

                blobList.Add(blobInd);
            }
            return blobList;
        }

        public async Task<string> GetBlob(string name, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(name);

            return blobClient.Uri.AbsoluteUri;

        }

        public async Task<bool> UploadBlob(string name, IFormFile file, string containerName, Blob blob)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(name);

            var httpHeaders = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };

            IDictionary<string, string> metaData = new Dictionary<string, string>();

            metaData.Add("title", blob.Title);
            metaData["comment"] = blob.Comment;

            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders, metaData);

            // Below code to remove meta-data
            //metaData.Remove("title");

            //await blobClient.SetMetadataAsync(metaData);

            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}
