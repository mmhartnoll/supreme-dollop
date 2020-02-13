using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MindSculptor.Tools.Applications.Harvester.Processing
{
    internal abstract class HtmlAdapter
    {
        protected HtmlNode HtmlNode { get; }

        protected HtmlAdapter(HtmlNode htmlNode)
            => HtmlNode = htmlNode;

        public static Task<HtmlNode> GetHtmlNodeAsync(string url)
        => GetHtmlNodeAsync(url, Encoding.UTF8);

        protected static async Task<HtmlNode> GetHtmlNodeAsync(string url, Encoding encoding)
        {
            var exceptions = new List<Exception>();
            for (int i = 0; i <= 5; i++)
                try
                {
                    var htmlDocument = new HtmlDocument();
                    using (var webClient = new WebClient())
                    {
                        webClient.Encoding = encoding;
                        var htmlString = await webClient.DownloadStringTaskAsync(url)
                            .ConfigureAwait(false);
                        htmlDocument.LoadHtml(htmlString);
                        return htmlDocument.DocumentNode;
                    }
                }
                catch (WebException ex)
                {
                    exceptions.Add(ex);
                    //var response = (HttpWebResponse)ex.Response;
                    //if (response != null && response.StatusCode == HttpStatusCode.InternalServerError)
                    //{
                        await Task.Delay(5000)
                            .ConfigureAwait(false);
                        continue;
                    //}
                    //break;
                }
            throw new AggregateException(exceptions);
        }
    }
}
