using System.Xml.Linq;


namespace cloudscribe.Syndication.Models.Rss
{
    public interface IXmlFormatter
    {
        XDocument BuildXml(RssChannel channel);
    }
}