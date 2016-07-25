using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.Syndication.Models.Rss
{
    public interface IChannelProvider
    {
        string Name { get; }
        
        Task<RssChannel> GetChannel(CancellationToken cancellationToken = default(CancellationToken));
    }
}
