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
        public void HTML����Topics�f�[�^���쐬���邱�Ƃ��ł��邱��()
        {
            var html = File.ReadAllText("./Resources/test-resource.html", System.Text.Encoding.UTF8);
            var parser = new HtmlParser();
            var docs = parser.ParseDocument(html);
            var resData = _ntaTopicsService.GenerateNtaTopicsData(docs);

            Assert.NotNull(resData);

            // Title
            var titleExists = resData.Where(w => w.Title == "�u�ߘa�T�N���@�m��\�����W�v���J�݂��܂���").Any();
            Assert.True(titleExists, "���Œ��̃g�s�b�N�X�ꗗHTML�̃^�C�g����title�p�����[�^�ƂȂ邱��");

            // Id
            var idExists = resData.Where(w => w.Id == "a0023012-266").Any();
            Assert.True(idExists, "���Œ��̃g�s�b�N�X�ꗗHTML��id��id�p�����[�^�ƂȂ邱��");

            // LinkUrl
            var linkText = resData.Where(w => w.TopicPage != null && w.TopicPage.ToString().Contains("/taxes/shiraberu/shinkoku/tokushu/index.htm"));
            var linkExists = linkText.Any();
            Assert.True(linkExists, "���Œ��̃g�s�b�N�X�ꗗHTML�̃^�C�g���̃����N�悪topicPage�p�����[�^�ƂȂ邱��");

            var originExists = linkText?.First()?.TopicPage?.OriginalString.Contains("https://www.nta.go.jp");
            Assert.True(linkExists, "���Œ��̃g�s�b�N�X�ꗗHTML�̃^�C�g���̃����N���origin���t�^����Ă��邱��");

            // PublishDate
            var publishDateExists = resData.Where(w => w.PublishDate == new DateTime(2024, 1, 4, 0, 0, 0)).Any();
            Assert.True(publishDateExists, "���Œ��̃g�s�b�N�X�ꗗHTML�̓��t��publishDate�p�����[�^�ƂȂ邱��");
        }

        [Fact]
        public void HTML���ɊY��Class�����݂��Ȃ��ꍇ��z�񂪕ԋp����邱��()
        {
            var html = File.ReadAllText("./Resources/class-not-exists.html", System.Text.Encoding.UTF8);
            var parser = new HtmlParser();
            var docs = parser.ParseDocument(html);
            var resData = _ntaTopicsService.GenerateNtaTopicsData(docs);
            Assert.Null(resData);
        }

        [Fact]
        public void HTML���̊Y��Class����tr�s�����݂��Ȃ��ꍇ��z�񂪕ԋp����邱��()
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
                { "�ߘa6�N1��6��", new DateTime(2024, 1, 6, 0, 0, 0) },
                { "�ߘa���N5��1��", new DateTime(2019, 5, 1, 0, 0, 0) },
                { "����31�N4��30��", new DateTime(2019, 4, 30, 0, 0, 0) },
                { "�������N1��8��", new DateTime(1989, 1, 8, 0, 0, 0) },
                { "���a64�N1��7��", new DateTime(1989, 1, 7, 0, 0, 0) }
            };

        [Theory]
        [MemberData(nameof(TestCase))]
        public void �a���ϊ����������s���邱��(string eraDate, DateTime date)
        {
            var res = _ntaTopicsService.EraToDate(eraDate);
            Assert.Equal(date, res);
        }

        [Fact]
        public void ���݂��Ȃ��a��w�肳�ꂽ�ꍇ��MinDate���w�肳��邱��()
        {
            var eraDate = "�l�I3�N1��1��";
            var res = _ntaTopicsService.EraToDate(eraDate);
            var expected = DateTime.MinValue;
            Assert.Equal(expected, res);
        }

        [Fact]
        public void �V�X�e�����F���ł��Ȃ��a��w�肳�ꂽ�ꍇ��MinDate���w�肳��邱��()
        {
            var eraDate = "�剻���N7��30��";
            var res = _ntaTopicsService.EraToDate(eraDate);
            var expected = DateTime.MinValue;
            Assert.Equal(expected, res);
        }

        [Fact]
        public void �g�s�b�N�s���ɕϊ��ł����񂪑��݂��Ȃ��ꍇ�͂��̍s��RSS�ɏo�͂���Ȃ�����()
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