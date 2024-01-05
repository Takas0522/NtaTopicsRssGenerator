using NtaTopicsRssGenerator.Models;
using NtaTopicsRssGenerator.Repositories;
using System;
using System.Collections.Generic;
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
        private readonly NtaTopicsRepository _ntaTopicsRepository;
        private readonly StorageRepository _storageRepository;
        public NtaTopicsService(
            NtaTopicsRepository repository,
            StorageRepository storageRepository
        )
        {
            _ntaTopicsRepository = repository;
            _storageRepository = storageRepository;
        }

        public async Task GenerateRss()
        {
            var topicsData = await _ntaTopicsRepository.GetTopics();
            var rssBaseData = topicsData.Take(20);
            var rssFeed = GenerateRssFeedData(rssBaseData);
            using var stream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(stream);
            rssFeed.WriteTo(xmlWriter);
            xmlWriter.Close();
            stream.Position = 0;
            await _storageRepository.SaveRssFeed(stream);
        }

        private static Rss20FeedFormatter GenerateRssFeedData(IEnumerable<NtaTopic> data)
        {
            var feed = new SyndicationFeed("国税庁トピック一覧RSS(非公式)", "国税庁のトピックス一覧の非公式RSS Feedです", new Uri("https://www.nta.go.jp/information/news/index.htm"));
            feed.Authors.Add(new SyndicationPerson("DevTakas"));
            var items = data.Select(s => {
                return new SyndicationItem(s.Title, s.Title, s.TopicPage, s.Id, s.PublishDate);
            });
            feed.Items = items;
            return new Rss20FeedFormatter(feed);
        }
    }
}
