using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtaTopicsRssGenerator.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly BlobServiceClient _client;
        private readonly string _blobContainerName = "rss-feed";
        private readonly string _blobName = "rss.xml";

        public StorageRepository(
            BlobServiceClient client,
            IConfiguration config
        )
        {
            _client = client;
            var containeName = config.GetValue<string>("CONTAINER_NAME");
            if (containeName != null)
            {
                _blobContainerName = containeName;
            }
            var blobName = config.GetValue<string>("BLOB_NAME");
            if (blobName != null)
            {
                _blobName = blobName;
            }
        }

        public async Task SaveRssFeedAsync(Stream rssFeed)
        {
            var containerClient = _client.GetBlobContainerClient(_blobContainerName);
            var blobClient = containerClient.GetBlobClient(_blobName);
            await blobClient.UploadAsync(rssFeed, overwrite: true);
        }
    }
}
