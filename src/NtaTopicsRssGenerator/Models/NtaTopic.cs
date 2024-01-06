using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtaTopicsRssGenerator.Models
{
    public class NtaTopic
    {
        public string Id { get; set; } = "";
        public DateTime PublishDate { get; set; } = DateTime.MinValue;
        public string Title { get; set; } = "";
        public Uri? TopicPage { get; set; }
    }
}
