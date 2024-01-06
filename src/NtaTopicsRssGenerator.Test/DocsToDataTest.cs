using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Configuration;
using NtaTopicsRssGenerator.Repositories;
using NtaTopicsRssGenerator.Services;
using PrivateProxy;

namespace NtaTopicsRssGenerator.Test
{
    public class DocsToDataTest
    {

        private readonly NtaTopicsService _ntaTopicsService;

        public DocsToDataTest()
        {
            var config = new ConfigurationBuilder().Build();
            _ntaTopicsService = new NtaTopicsService(new DummyNtaTopicsRepository(), new DummyStorageRepository(), config);
        }

        [Fact]
        public void HTMLからTopicsデータを作成することができること()
        {
            var html = File.ReadAllText("./Resources/test-resource.html", System.Text.Encoding.UTF8);
            var parser = new HtmlParser();
            var docs = parser.ParseDocument(html);
            var resData = _ntaTopicsService.GenerateNtaTopicsData(docs);

            Assert.NotNull(resData);

            // Title
            var titleExists = resData.Where(w => w.Title == "「令和５年分　確定申告特集」を開設しました").Any();
            Assert.True(titleExists, "国税庁のトピックス一覧HTMLのタイトルがtitleパラメータとなること");

            // Id
            var idExists = resData.Where(w => w.Id == "a0023012-266").Any();
            Assert.True(idExists, "国税庁のトピックス一覧HTMLのidがidパラメータとなること");

            // LinkUrl
            var linkText = resData.Where(w => w.TopicPage != null && w.TopicPage.ToString().Contains("/taxes/shiraberu/shinkoku/tokushu/index.htm"));
            var linkExists = linkText.Any();
            Assert.True(linkExists, "国税庁のトピックス一覧HTMLのタイトルのリンク先がtopicPageパラメータとなること");

            var originExists = linkText?.First()?.TopicPage?.OriginalString.Contains("https://www.nta.go.jp");
            Assert.True(linkExists, "国税庁のトピックス一覧HTMLのタイトルのリンク先にoriginが付与されていること");

            // PublishDate
            var publishDateExists = resData.Where(w => w.PublishDate == new DateTime(2024, 1, 4, 0, 0, 0)).Any();
            Assert.True(publishDateExists, "国税庁のトピックス一覧HTMLの日付がpublishDateパラメータとなること");
        }

        [Fact]
        public void HTML内に該当Classが存在しない場合空配列が返却されること()
        {
            var html = File.ReadAllText("./Resources/class-not-exists.html", System.Text.Encoding.UTF8);
            var parser = new HtmlParser();
            var docs = parser.ParseDocument(html);
            var resData = _ntaTopicsService.GenerateNtaTopicsData(docs);
            Assert.Null(resData);
        }

        [Fact]
        public void HTML内の該当Class内にtr行が存在しない場合空配列が返却されること()
        {
            
            var html = File.ReadAllText("./Resources/tr-not-exists.html", System.Text.Encoding.UTF8);
            var parser = new HtmlParser();
            var docs = parser.ParseDocument(html);
            var resData = _ntaTopicsService.GenerateNtaTopicsData(docs);
            Assert.NotNull(resData);
            Assert.False(resData.Any());
        }

        public static TheoryData<string, DateTime> TestCase =
            new TheoryData<string, DateTime>() {
                { "令和6年1月6日", new DateTime(2024, 1, 6, 0, 0, 0) },
                { "令和元年5月1日", new DateTime(2019, 5, 1, 0, 0, 0) },
                { "平成31年4月30日", new DateTime(2019, 4, 30, 0, 0, 0) },
                { "平成元年1月8日", new DateTime(1989, 1, 8, 0, 0, 0) },
                { "昭和64年1月7日", new DateTime(1989, 1, 7, 0, 0, 0) }
            };

        [Theory]
        [MemberData(nameof(TestCase))]
        public void 和暦西暦変換が正しく行われること(string eraDate, DateTime date)
        {
            var res = _ntaTopicsService.EraToDate(eraDate);
            Assert.Equal(date, res);
        }

        [Fact]
        public void 存在しない和暦が指定された場合はMinDateが指定されること()
        {
            var eraDate = "ネオ3年1月1日";
            var res = _ntaTopicsService.EraToDate(eraDate);
            var expected = DateTime.MinValue;
            Assert.Equal(expected, res);
        }

        [Fact]
        public void システムが認識できない和暦が指定された場合はMinDateが指定されること()
        {
            var eraDate = "大化元年7月30日";
            var res = _ntaTopicsService.EraToDate(eraDate);
            var expected = DateTime.MinValue;
            Assert.Equal(expected, res);
        }

        [Fact]
        public void トピック行内に変換できる情報が存在しない場合はその行はRSSに出力されないこと()
        {

            var html = File.ReadAllText("./Resources/failed-row.html", System.Text.Encoding.UTF8);
            var parser = new HtmlParser();
            var docs = parser.ParseDocument(html);
            var resData = _ntaTopicsService.GenerateNtaTopicsData(docs);
            Assert.NotNull(resData);
            var cn = resData.Count();
            Assert.Equal(1, cn);
        }

    }

}