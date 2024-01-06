using AngleSharp.Dom;
using Microsoft.Extensions.Configuration;
using NtaTopicsRssGenerator.Models;
using NtaTopicsRssGenerator.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NtaTopicsRssGenerator.Services
{
    public class NtaTopicsService
    {

        private readonly INtaTopicsRepository _ntaTopicsRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly CultureInfo _culture;
        private readonly string _ntaUrl = "https://www.nta.go.jp";

        public NtaTopicsService(
            INtaTopicsRepository repository,
            IStorageRepository storageRepository,
            IConfiguration config
        )
        {
            _ntaTopicsRepository = repository;
            _storageRepository = storageRepository;
            _culture = new CultureInfo("ja-JP", true);
            _culture.DateTimeFormat.Calendar = new JapaneseCalendar();
            var domain = config.GetValue<string>("NTA_ORIGIN");
            if (domain != null)
            {
                _ntaUrl = domain;
            }
        }

        public async Task GenerateRssAsync()
        {
            var topicsDocs = await _ntaTopicsRepository.GetTopicsAsync();
            var rssBaseData = GenerateNtaTopicsData(topicsDocs);
            if (rssBaseData == null)
            {
                return;
            }
            var rssFeed = GenerateRssFeedData(rssBaseData);
            await SaveXmlAsync(rssFeed);
        }

        private async Task SaveXmlAsync(Rss20FeedFormatter rssFeed)
        {
            using var stream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(stream);
            rssFeed.WriteTo(xmlWriter);
            xmlWriter.Close();
            stream.Position = 0;
            await _storageRepository.SaveRssFeedAsync(stream);
        }

        // TODO: .NET8対応後にPrivateにしてPrivateProxy経由でテストしたい
        internal IEnumerable<NtaTopic>? GenerateNtaTopicsData(IDocument docs)
        {
            var genData = new List<NtaTopic>();
            var tables = docs.GetElementsByClassName("index_news");
            if (tables == null || !tables.Any())
            {
                return null;
            }
            foreach (var table in tables)
            {
                var linedatas = table.QuerySelectorAll("tr");
                if (linedatas != null && linedatas.Any())
                {
                    var ntaTopics = linedatas.Select(s => {
                        var dateJp =  s.QuerySelector("th")?.TextContent;
                        var date = (dateJp != null) ? EraToDate(dateJp): DateTime.MinValue;
                        var link = s.QuerySelector("a")?.GetAttribute("href");
                        var title = s.QuerySelector("a")?.TextContent ?? "";
                        var id = s.GetAttribute("id") ?? "";
                        if (link != null && !link.Contains(_ntaUrl))
                        {
                            link = _ntaUrl + link;
                        }
                        return new NtaTopic
                        {
                            Id = id,
                            PublishDate = date,
                            Title = title,
                            TopicPage = (link != null) ? new Uri(link) : null
                        };
                    });
                    genData.AddRange(ntaTopics);
                }
            }
            var resData = genData.Where(w =>{
                return (w.Id != "" && w.PublishDate != DateTime.MinValue && w.Title != "" && w.TopicPage != null);
            });
            return resData;
        }

        // TODO: .NET8対応後にPrivateにしてPrivateProxy経由でテストしたい
        internal DateTime EraToDate(string eraDate)
        {
            var canParse = DateTime.TryParseExact(eraDate, "ggyy年M月d日", _culture, DateTimeStyles.AssumeLocal, out DateTime date);
            if (canParse)
            {
                return date;
            }
            return DateTime.MinValue;
        }

        // TODO: .NET8対応後にPrivateにしてPrivateProxy経由でテストしたい
        internal static Rss20FeedFormatter GenerateRssFeedData(IEnumerable<NtaTopic> data)
        {
            var rssBaseData = data.Take(20);
            var feed = new SyndicationFeed("国税庁トピック一覧RSS(非公式)", "国税庁のトピックス一覧の非公式RSS Feedです", new Uri("https://www.nta.go.jp/information/news/index.htm"));
            feed.Authors.Add(new SyndicationPerson("", "DevTakas", ""));
            var items = rssBaseData.Select(s => {
                return new SyndicationItem(s.Title, s.Title, s.TopicPage, s.Id, s.PublishDate);
            });
            feed.Items = items;
            return new Rss20FeedFormatter(feed);
        }
    }
}
