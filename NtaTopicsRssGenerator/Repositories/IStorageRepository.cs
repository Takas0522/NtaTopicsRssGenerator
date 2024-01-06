using System.IO;
using System.Threading.Tasks;

namespace NtaTopicsRssGenerator.Repositories
{
    public interface IStorageRepository
    {
        Task SaveRssFeedAsync(Stream rssFeed);
    }
}