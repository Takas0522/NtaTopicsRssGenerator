using AngleSharp;
using NtaTopicsRssGenerator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtaTopicsRssGenerator.Repositories
{
    public class NtaTopicsRepository
    {
        private readonly IBrowsingContext _context;
        private readonly string _ntaTopicUrl = "https://www.nta.go.jp/information/news/index.htm";
        private readonly CultureInfo _culture;
        private readonly string _ntaUrl = "https://www.nta.go.jp";
        public NtaTopicsRepository()
        {
            var config = Configuration.Default.WithDefaultLoader();
            _context = BrowsingContext.New(config);
            _culture = new CultureInfo("ja-JP", true);
            _culture.DateTimeFormat.Calendar = new JapaneseCalendar();
        }

        public async Task<IEnumerable<NtaTopic>> GetTopics()
        {

            var resData = new List<NtaTopic>();
            var docs = await _context.OpenAsync(_ntaTopicUrl);
            var tables = docs.GetElementsByClassName("index_news");
            foreach ( var table in tables )
            {
                var linedatas = table.QuerySelectorAll("tr");
                var ntaTopics = linedatas.Select(s => {
                    var dateJp = s.QuerySelector("th").TextContent;;
                    var date = DateTime.ParseExact(dateJp, "ggyy年M月d日", _culture, DateTimeStyles.AssumeLocal);
                    var link = s.QuerySelector("a").GetAttribute("href");
                    var title = s.QuerySelector("a").TextContent;
                    var id = s.GetAttribute("id");
                    if (!link.Contains(_ntaUrl))
                    {
                        link = _ntaUrl + link;
                    }

                    return new NtaTopic {
                        Id = id,
                        PublishDate = date,
                        Title = title,
                        TopicPage = new Uri(link)
                    };
                });
                resData.AddRange(ntaTopics);
            }
            return resData;
        }
    }
}
