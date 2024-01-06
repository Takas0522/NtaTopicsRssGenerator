using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using NtaTopicsRssGenerator;
using NtaTopicsRssGenerator.Repositories;
using NtaTopicsRssGenerator.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NtaTopicsRssGenerator
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();
            builder.Services.AddSingleton<INtaTopicsRepository, NtaTopicsRepository>();
            builder.Services.AddSingleton<NtaTopicsService>();
            builder.Services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(configuration.GetConnectionString("BlobStorage"));
            });
            builder.Services.AddSingleton<IStorageRepository, StorageRepository>();
        }
    }
}
