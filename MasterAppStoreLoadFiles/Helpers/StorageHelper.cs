namespace MasterAppStoreLoadFiles.Helpers
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using MasterAppStoreLoadFiles.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Flurl;
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using System;
    using System.IO;

    public static class StorageHelper
    {
        private static async Task<BlobContainerClient> GetCloudBlobContainer(string containerName, string connectionString)
        {
            BlobServiceClient serviceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }

        public static bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<List<string>> GetImagesUrls(AzureStorageConfig storaConfig)
        {
            BlobContainerClient containerClient = await GetCloudBlobContainer(storaConfig.ImageContainer, storaConfig.ConnectionString);
            List<string> results = new List<string>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                results.Add(
                    Flurl.Url.Combine(
                        containerClient.Uri.AbsoluteUri,
                        blobItem.Name
                        )
                    );
            }
            return results;
        }

        public static async Task<bool> UploadFileToStorage(Stream fileStream, string fileName, AzureStorageConfig storageConfig, string idProduct)
        {
            BlobContainerClient containerClient = await GetCloudBlobContainer(storageConfig.ImageContainer, storageConfig.ConnectionString);
            string blobName = string.Concat(Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty), Path.GetExtension(fileName));
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream);
            await SetMetadata(blobClient, idProduct);
            return await Task.FromResult(true);
        }

        public static async Task<bool> SetMetadata(BlobClient blobClient, string idProduct)
        {
            string p_key = "ProductId";
            string p_value = idProduct;
            IDictionary<string, string> objProdduct = new Dictionary<string, string>();
            objProdduct.Add(p_key, p_value);
            blobClient.SetMetadata(objProdduct);

            return await Task.FromResult(true);

        }
        
}
}
