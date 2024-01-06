using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Configuration;
using NtaTopicsRssGenerator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtaTopicsRssGenerator.Repositories
{
    public class NtaTopicsRepository : INtaTopicsRepository
    {
        private readonly IBrowsingContext _context;
        private readonly string _ntaTopicUrl = "https://www.nta.go.jp/information/news/index.htm";

        public NtaTopicsRepository(
            Microsoft.Extensions.Configuration.IConfiguration config
        )
        {
            var angleConfig = Configuration.Default.WithDefaultLoader();
            _context = BrowsingContext.New(angleConfig);
            var domain = config.GetValue<string>("NTA_ORIGIN");
            var endpoint = config.GetValue<string>("NTA_ENDPOINT");
            if (domain != null && endpoint != null)
            {
                _ntaTopicUrl = domain + endpoint;
            }
        }

        public async Task<IDocument> GetTopicsAsync()
        {

            var docs = await _context.OpenAsync(_ntaTopicUrl);
            return docs;
        }
    }
}
