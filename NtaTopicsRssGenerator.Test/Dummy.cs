using AngleSharp.Dom;
using Microsoft.Extensions.Configuration;
using NtaTopicsRssGenerator.Repositories;
using NtaTopicsRssGenerator.Services;
using PrivateProxy;

namespace NtaTopicsRssGenerator.Test
{
    internal class DummyStorageRepository : IStorageRepository
    {
        public Task SaveRssFeedAsync(Stream rssFeed)
        {
            throw new NotImplementedException();
        }
    }

    internal class DummyNtaTopicsRepository : INtaTopicsRepository
    {
        public Task<IDocument> GetTopicsAsync()
        {
            throw new NotImplementedException();
        }
    }

    // .NET8‘Î‰žŒã
    //[GeneratePrivateProxy(typeof(NtaTopicsService))]
    //public partial class NtaTopicsServiceProxy { }

}