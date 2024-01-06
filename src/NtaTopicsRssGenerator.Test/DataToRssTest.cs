using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Configuration;
using NtaTopicsRssGenerator.Models;
using NtaTopicsRssGenerator.Repositories;
using NtaTopicsRssGenerator.Services;
using PrivateProxy;

namespace NtaTopicsRssGenerator.Test
{
    public class DataToRssTest
    {

        private readonly NtaTopicsService _ntaTopicsService;

        public DataToRssTest()
        {
            var config = new ConfigurationBuilder().Build();
            _ntaTopicsService = new NtaTopicsService(new DummyNtaTopicsRepository(), new DummyStorageRepository(), config);
        }

        [Fact]
        public void Topics�f�[�^����RSS���������쐬����邱��()
        {
            var html = File.ReadAllText("./Resources/test-resource.html", System.Text.Encoding.UTF8);
            var parser = new HtmlParser();
            var docs = parser.ParseDocument(html);
            var resData = _ntaTopicsService.GenerateNtaTopicsData(docs);
            Assert.NotNull(resData);

            var rssData = NtaTopicsService.GenerateRssFeedData(resData);

            // Title
            Assert.Equal("���Œ��g�s�b�N�ꗗRSS(�����)", rssData.Feed.Title.Text.ToString());

            // Author
            Assert.Equal("DevTakas", rssData.Feed.Authors.First().Name);

            // item count
            Assert.Equal(20, rssData.Feed.Items.Count());

            // item
            // title
            var title = resData.First().Title;
            var isTitleExists = rssData.Feed.Items.Where(w => w.Title.Text == title).Any();
            Assert.True(isTitleExists, "�A�C�e����title�̓f�[�^��title���ڂƈ�v���邱��");

            // topicpage
            var link = resData.First().TopicPage;
            var isTopicPageExists = rssData.Feed.Items.Where(w => w.Links.First().Uri == link).Any();
            Assert.True(isTopicPageExists, "�A�C�e����itemAltenateLink�̓f�[�^��TopicPage���ڂƈ�v���邱��");

            var id = resData.First().Id;
            var isIdExists = rssData.Feed.Items.Where(w => w.Id == id).Any();
            Assert.True(isIdExists, "�A�C�e����id�̓f�[�^��Id���ڂƈ�v���邱��");

            var lastUpdateDate = resData.First().PublishDate;
            var isLastUpdateDateExists = rssData.Feed.Items.Where(w => w.LastUpdatedTime == lastUpdateDate).Any();
            Assert.True(isLastUpdateDateExists, "�A�C�e����lastUpdatedTime�̓f�[�^��PublishDate���ڂƈ�v���邱��");
        }


        [Fact]
        public void �f�[�^�̑�����20���ɖ����Ȃ��ꍇ�͂��̌����̃f�[�^���\������邱��()
        {
            var data = new List<NtaTopic> {
                new NtaTopic { Id = "1", PublishDate = DateTime.Now, Title = "a", TopicPage = null },
                new NtaTopic { Id = "2", PublishDate = DateTime.Now, Title = "b", TopicPage = null },
            };
            var rssData = NtaTopicsService.GenerateRssFeedData(data);
            // item count
            Assert.Equal(2, rssData.Feed.Items.Count());
        }


    }

}