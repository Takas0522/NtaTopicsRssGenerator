using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using NtaTopicsRssGenerator.Repositories;
using NtaTopicsRssGenerator.Services;

namespace NtaTopicsRssGenerator
{
    public class NtaTopicsRss
    {
        private readonly NtaTopicsService _service;
        public NtaTopicsRss(
            NtaTopicsService service
        )
        {
            _service = service;
        }

        [FunctionName("Generate")]
        [FixedDelayRetry(3, "00:01:00")]
        public async Task Run([TimerTrigger("0 0 0 * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await _service.GenerateRss();
        }
    }
}
