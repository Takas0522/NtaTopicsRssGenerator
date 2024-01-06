using AngleSharp.Dom;
using System.Threading.Tasks;

namespace NtaTopicsRssGenerator.Repositories
{
    public interface INtaTopicsRepository
    {
        Task<IDocument> GetTopicsAsync();
    }
}