using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MindSculptor.Tools.Applications.Harvester.Processing.Gatherer
{
    internal class GathererSetAdapter
    {
        private readonly AsyncLazy<IEnumerable<(string, int)>> nameIdPairsLoader;
        private readonly AsyncCachedLookup<(string, int), GathererCardAdapter> cardAdapterLookup;

        public string SetName { get; }

        public IAsyncEnumerable<GathererCardAdapter> Cards => StreamCardAdapters();

        private GathererSetAdapter(string setName)
        {
            SetName = setName;

            nameIdPairsLoader = AsyncLazy<IEnumerable<(string, int)>>.Create(LoadNameIdPairsAsync);
            cardAdapterLookup = AsyncCachedLookup<(string, int), GathererCardAdapter>.Create(LoadCardAdapterAsync);
        }

        public static GathererSetAdapter Create(string setName)
            => new GathererSetAdapter(setName);

        public async Task<int> GetCardCountAsync()
        {
            var nameIdPairs = await GetNameIdPairsAsync()
                .ConfigureAwait(false);
            return nameIdPairs.Count();
        }

        public async Task<GathererCardAdapter> FindAdapterByNameWithClosestMultiverseId(string name, int multiverseId)
        {
            var nameIdPairs = await GetNameIdPairsAsync()
                .ConfigureAwait(false);
            var minDiff = nameIdPairs
                .Where(nameIdPair => nameIdPair.Name == name)
                .Min(nameIdPair => nameIdPair.MultiverseId - multiverseId);
            var key = (name, minDiff + multiverseId);
            return await cardAdapterLookup.GetValueAsync(key)
                .ConfigureAwait(false);
        }

        private async IAsyncEnumerable<GathererCardAdapter> StreamCardAdapters()
        {
            var nameIdPairs = await GetNameIdPairsAsync()
                .ConfigureAwait(false);
            foreach (var nameIdPair in nameIdPairs)
                yield return await cardAdapterLookup.GetValueAsync(nameIdPair)
                    .ConfigureAwait(false);
        }

        private async Task<IEnumerable<(string Name, int MultiverseId)>> GetNameIdPairsAsync()
            => await nameIdPairsLoader;

        private async Task<IEnumerable<(string, int)>> LoadNameIdPairsAsync()
        {
            var nameIdPairs = new HashSet<(string, int)>();

            // Need to do a seperate search for 'Conspiracy' type as they do not show unless specified.
            await ProcessUrl($@"http://gatherer.wizards.com/Pages/Search/Default.aspx?output=compact&set=[""{SetName}""]&type=[""Conspiracy""]&sort=cn+")
                .ConfigureAwait(false);
            await ProcessUrl($@"http://gatherer.wizards.com/Pages/Search/Default.aspx?output=compact&set=[""{SetName}""]&sort=cn+")
                .ConfigureAwait(false);

            return nameIdPairs.Enumerate();

            async Task ProcessUrl(string url)
            {
                var pageIndex = 0;
                while (true)
                {
                    var pagedUrl = string.Join("&", url, $@"page={pageIndex}");
                    var htmlNode = await HtmlAdapter.GetHtmlNodeAsync(pagedUrl)
                        .ConfigureAwait(false);
                    var newNameIdPairs = htmlNode
                        .QuerySelectorAll(".cardItem")
                        .SelectMany(GetNameIdPairs)
                        .Where(nameIdPair => !nameIdPairs.Contains(nameIdPair))
                        .ToList();
                    if (!newNameIdPairs.Any())
                        return;
                    foreach (var nameIdPair in newNameIdPairs)
                        nameIdPairs.Add(nameIdPair);
                    pageIndex++;
                }
            }
        }

        private static async Task<VerifiedResult<GathererCardAdapter>> LoadCardAdapterAsync((string Name, int MultiverseId) key)
            => await GathererCardAdapter.LoadAsync(key.Name, key.MultiverseId).ConfigureAwait(false);

        private static IEnumerable<(string, int)> GetNameIdPairs(HtmlNode htmlNode)
        {
            var rawCardName = htmlNode.QuerySelector(".name a").InnerHtml;
            var cardName = Regex.Replace(rawCardName, @".*\((.*)\)", "$1");

            var rawLinks = htmlNode.QuerySelectorAll(".printings div a")
                .Select(node => node.GetAttributeValue("href", null));
            var multiverseIds = rawLinks
                .Select(link => Regex.Replace(link, @".*multiverseid=(\d+)", "$1"))
                .Select(multiverseId => Convert.ToInt32(multiverseId));

            foreach (var multiverseId in multiverseIds)
                yield return (cardName, multiverseId);
        }
    }
}
