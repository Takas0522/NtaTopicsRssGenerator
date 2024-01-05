using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtaTopicsRssGenerator.Repositories
{
    public class StorageRepository
    {
        private readonly BlobServiceClient _client;

        public StorageRepository(
            BlobServiceClient client
        )
        {
            _client = client;
        }

        public async Task SaveRssFeed(Stream rssFeed)
        {
            var containerClient = _client.GetBlobContainerClient("rss-feed");
            var blobClient = containerClient.GetBlobClient("rss.xml");
            await blobClient.UploadAsync(rssFeed, overwrite: true);
        }
    }
}
