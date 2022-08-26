using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using MindSculptor.Tools.Data;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.Tools.Applications.Harvester.Processing.Gatherer
{
    internal class GathererCardDataMapper
    {
        private readonly DataContext dataContext;
        private readonly IProgress<Progress> progress;

        private readonly Lazy<Task<IReadOnlyDictionary<string, ColorIdentity>>> colorIdentityLookupLoader;

        private readonly Lazy<Task<IReadOnlyCollection<string>>> superTypesLoader;
        private readonly Lazy<Task<IReadOnlyCollection<string>>> mainTypesLoader;
        private readonly Lazy<Task<IReadOnlyCollection<string>>> subTypesLoader;

        private readonly Lazy<Task<IEnumerable<(string, int)>>> nameIdPairsLoader;

        private readonly string setCode;
        private readonly string? setCodeExtension;
        private readonly string setName;

        private Task<IReadOnlyDictionary<string, ColorIdentity>> ColorIdentityLookup => colorIdentityLookupLoader.Value;
        private Task<IReadOnlyCollection<string>> SuperTypes => superTypesLoader.Value;
        private Task<IReadOnlyCollection<string>> MainTypes => mainTypesLoader.Value;
        private Task<IReadOnlyCollection<string>> SubTypes => subTypesLoader.Value;

        private Task<IEnumerable<(string Name, int MultiverseId)>> NameIdPairs => nameIdPairsLoader.Value;

        public GathererCardDataMapper(DataContext dataContext, IProgress<Progress> progress, string setCode, string? setCodeExtension, string setName)
        {
            this.dataContext        = dataContext;
            this.progress           = progress;
            this.setCode            = setCode;
            this.setCodeExtension   = setCodeExtension;
            this.setName            = setName;

            colorIdentityLookupLoader = new Lazy<Task<IReadOnlyDictionary<string, ColorIdentity>>>(LoadColorIdentityLookup);

            superTypesLoader = new Lazy<Task<IReadOnlyCollection<string>>>(LoadSuperTypes);
            mainTypesLoader = new Lazy<Task<IReadOnlyCollection<string>>>(LoadMainTypes);
            subTypesLoader = new Lazy<Task<IReadOnlyCollection<string>>>(LoadSubTypes);

            nameIdPairsLoader = new Lazy<Task<IEnumerable<(string, int)>>>(LoadNameIdPairsAsync);
        }

        public async Task<IEnumerable<Card>> LoadAllCardData()
        {
            var loadedCount = 0;
            var totalCount = (await NameIdPairs.ConfigureAwait(false)).Count();

            var cards = new List<Card>();
            foreach (var nameIdPair in await NameIdPairs.ConfigureAwait(false))
            {
                var cardAdapters = new List<GathererCardAdapter>() { await GathererCardAdapter.LoadAsync(nameIdPair.Name, nameIdPair.MultiverseId).ConfigureAwait(false) };
                var linkedCardResult = await LoadLinkedCardAdapter(cardAdapters.Single()).ConfigureAwait(false);
                if (linkedCardResult.Success)
                    cardAdapters.Add(linkedCardResult.Value);

                ReportProgress(++loadedCount, totalCount, string.Join(" // ", cardAdapters.OrderByDescending(adapter => adapter.IsPrimaryFace).Select(adapter => adapter.Name)) + $" ({loadedCount}/{totalCount})");

                var primaryCardAdapter = linkedCardResult.Success ?
                    cardAdapters.Single(adapter => adapter.IsPrimaryFace) : cardAdapters.Single();
                var primaryCardFace = await MapCardFaceData(primaryCardAdapter).ConfigureAwait(false);
                var secondaryCardFace = linkedCardResult.Success ?
                    await MapCardFaceData(cardAdapters.Single(adapter => !adapter.IsPrimaryFace)).ConfigureAwait(false) : null;

                var cardType = DetermineCardType(linkedCardResult.Success, primaryCardFace);
                var collectorsNumber = primaryCardAdapter.HasCollectorsNumber ? primaryCardAdapter.CollectorsNumber : (int?)null;

                cards.Add(new Card(cardType, "Main Set", "Regular", setCode, setCodeExtension, collectorsNumber, primaryCardAdapter.Rarity, primaryCardAdapter.Artist, primaryCardFace, secondaryCardFace, null));
            }
            return (await OrderCards(cards).ConfigureAwait(false)).Enumerate();
        }

        private string DetermineCardType(bool isLinkedCard, CardFace primaryCardFace)
        {
            if (!isLinkedCard)
                return "SingleFaced";
            if (primaryCardFace.HasOracleText)
            {
                if (primaryCardFace.OracleText.ToLowerInvariant().Contains("transform"))
                    return "Transform";
                if (primaryCardFace.OracleText.ToLowerInvariant().Contains("meld"))
                    return "Meld";
                if (primaryCardFace.OracleText.ToLowerInvariant().Contains("flip"))
                    return "Flip";
                if (primaryCardFace.OracleText.ToLowerInvariant().Contains("fuse"))
                    return "Split";
            }
            if (setCode == "AKH" || setCode == "HOU")
                return "Aftermath";
            if (setCode == "GRN" || setCode == "RNA")
                return "Split";
            if (setCode == "ELD")
                return "Adventure";
            return "Uncategorized";
        }

        private async Task<VerifiedResult<GathererCardAdapter>> LoadLinkedCardAdapter(GathererCardAdapter cardAdapter)
        {
            if (!(cardAdapter is GathererMultiFaceCardAdapter multiFaceCardAdapter))
                return VerifiedResult<GathererCardAdapter>.Failure;

            // Gatherer displays 'Battlebond' partner cards as a pair, but these should be treated differently
            if (setName == "Battlebond")
                return VerifiedResult<GathererCardAdapter>.Failure;

            var closestPair = await FindClosestNameIdPair(multiFaceCardAdapter.LinkedFaceName, multiFaceCardAdapter.MultiverseId).ConfigureAwait(false);
            var linkedCardAdapter = await GathererCardAdapter.LoadAsync(closestPair.Name, closestPair.MultiverseId).ConfigureAwait(false);

            // Gatherer displays both meld cards as linked cards, but we link only those with a matching collector's number
            if (cardAdapter.CollectorsNumber != linkedCardAdapter.CollectorsNumber)
                return VerifiedResult<GathererCardAdapter>.Failure;

            return VerifiedResult<GathererCardAdapter>.Successful(linkedCardAdapter);
        }

        private async Task<CardFace> MapCardFaceData(GathererCardAdapter cardAdapter)
        {
            var knownSuperTypes = await SuperTypes.ConfigureAwait(false);
            var knownMainTypes = await MainTypes.ConfigureAwait(false);
            var knownSubTypes = await SubTypes.ConfigureAwait(false);

            var superTypes = cardAdapter.Types.Where(type => knownSuperTypes.Contains(type));
            var mainTypes = cardAdapter.Types.Where(type => knownMainTypes.Contains(type));
            var subTypes = cardAdapter.Types.Where(type => knownSubTypes.Contains(type));

            var identifiedTypes = superTypes.Union(mainTypes).Union(subTypes);
            var missingTypes = cardAdapter.Types.Except(identifiedTypes);
            if (missingTypes.Any())
            {
                var formattedMissingTypes = string.Join(", ", missingTypes.Select(type => $"'{type}'"));
                var s = missingTypes.Count() > 1 ? "s" : string.Empty;
                throw new InvalidOperationException($"The type{s} {formattedMissingTypes} could not be found in the database. You may need to add the record{s} manually.");
            }

            var oracleText = cardAdapter.HasOracleText ? cardAdapter.OracleText : null;
            var power = cardAdapter.HasPowerAndToughness ? cardAdapter.Power : null;
            var toughness = cardAdapter.HasPowerAndToughness ? cardAdapter.Toughness : null;
            var loyalty = cardAdapter.HasLoyalty ? cardAdapter.Loyalty : null;
            var flavorText = cardAdapter.HasFlavorText ? cardAdapter.FlavorText : null;

            return new CardFace(cardAdapter.Name, cardAdapter.CastingCost, superTypes, mainTypes, subTypes, oracleText, power, toughness, loyalty, flavorText);
        }

        private async Task<IEnumerable<Card>> OrderCards(IEnumerable<Card> cards)
        {
            if (cards.Any(card => !card.HasCollectorsNumber))
            {
                var colorIdentityLookup = await ColorIdentityLookup.ConfigureAwait(false);
                IEnumerable<(Card Card, ColorIdentity ColorIdentity)> cardIdentityPairs = cards.Select(card => 
                {
                    var colorIdentity = ColorIdentity.Colorless;
                    foreach (var manaSymbol in card.PrimaryCardFace.CastingCost)
                        colorIdentity |= colorIdentityLookup[manaSymbol.SymbolType];
                    if (card.HasSecondaryCardFace)
                        foreach (var manaSymbol in card.SecondaryCardFace.CastingCost)
                            colorIdentity |= colorIdentityLookup[manaSymbol.SymbolType];
                    return (card, colorIdentity);
                });

                return OrderByColorIdentitiesAndTypes(cardIdentityPairs);
            }
            else
                return cards.OrderBy(card => card.CollectorsNumber).Enumerate();

            IEnumerable<Card> OrderByColorIdentitiesAndTypes(IEnumerable<(Card Card, ColorIdentity ColorIdentity)> cardIdentityPairs)
            {
                var colorlessCards = cardIdentityPairs.Where(pair => pair.ColorIdentity == ColorIdentity.Colorless)
                    .Select(pair => pair.Card)
                    .OrderBy(card => card.PrimaryCardFace.Name);
                var whiteCards = cardIdentityPairs.Where(pair => pair.ColorIdentity == ColorIdentity.White)
                    .Select(pair => pair.Card)
                    .OrderBy(card => card.PrimaryCardFace.Name);
                var blueCards = cardIdentityPairs.Where(pair => pair.ColorIdentity == ColorIdentity.Blue)
                    .Select(pair => pair.Card)
                    .OrderBy(card => card.PrimaryCardFace.Name);
                var blackCards = cardIdentityPairs.Where(pair => pair.ColorIdentity == ColorIdentity.Black)
                    .Select(pair => pair.Card)
                    .OrderBy(card => card.PrimaryCardFace.Name);
                var redCards = cardIdentityPairs.Where(pair => pair.ColorIdentity == ColorIdentity.Red)
                    .Select(pair => pair.Card)
                    .OrderBy(card => card.PrimaryCardFace.Name);
                var greenCards = cardIdentityPairs.Where(pair => pair.ColorIdentity == ColorIdentity.Green)
                    .Select(pair => pair.Card)
                    .OrderBy(card => card.PrimaryCardFace.Name);
                var multiColorCards = cards.Except(colorlessCards).Except(whiteCards).Except(blueCards).Except(blackCards).Except(redCards).Except(greenCards)
                    .OrderBy(card => card.HasSecondaryCardFace).ThenBy(card => card.PrimaryCardFace.Name);

                foreach (var card in colorlessCards.Where(card => !card.PrimaryCardFace.MainTypes.Contains("Artifact") && !card.PrimaryCardFace.MainTypes.Contains("Land")))
                    yield return card;
                foreach (var card in whiteCards) yield return card;
                foreach (var card in blueCards) yield return card;
                foreach (var card in blackCards) yield return card;
                foreach (var card in redCards) yield return card;
                foreach (var card in greenCards) yield return card;
                foreach (var card in multiColorCards) yield return card;
                foreach (var card in colorlessCards.Where(card => card.PrimaryCardFace.MainTypes.Contains("Artifact") && !card.PrimaryCardFace.MainTypes.Contains("Land")))
                    yield return card;
                foreach (var card in colorlessCards.Where(card => card.PrimaryCardFace.MainTypes.Contains("Land") && !card.PrimaryCardFace.SuperTypes.Contains("Basic")))
                    yield return card;
                foreach (var card in colorlessCards.Where(card => card.PrimaryCardFace.MainTypes.Contains("Land") && card.PrimaryCardFace.SuperTypes.Contains("Basic")))
                    yield return card;
            }
        }

        private async Task<IReadOnlyDictionary<string, ColorIdentity>> LoadColorIdentityLookup()
        {
            var colorIdentityLookup = new Dictionary<string, ColorIdentity>();
            await foreach (var record in dataContext.Cards.ManaSymbols.ConfigureAwait(false))
            {
                var colorIdentity = ColorIdentity.Colorless;
                colorIdentity |= record.HasWhiteIdentity    ? ColorIdentity.White   : 0;
                colorIdentity |= record.HasBlueIdentity     ? ColorIdentity.Blue    : 0;
                colorIdentity |= record.HasBlackIdentity    ? ColorIdentity.Black   : 0;
                colorIdentity |= record.HasRedIdentity      ? ColorIdentity.Red     : 0;
                colorIdentity |= record.HasGreenIdentity    ? ColorIdentity.Green   : 0;

                colorIdentityLookup.Add(record.Code, colorIdentity);
            }
            return new ReadOnlyDictionary<string, ColorIdentity>(colorIdentityLookup);
        }

        private async Task<IReadOnlyCollection<string>> LoadSuperTypes()
        {
            var hashSet = new HashSet<string>();
            await foreach (var record in dataContext.Cards.SuperTypes.ConfigureAwait(false))
                hashSet.Add(record.Value);
            return hashSet.ToImmutableHashSet();
        }

        private async Task<IReadOnlyCollection<string>> LoadMainTypes()
        {
            var hashSet = new HashSet<string>();
            await foreach (var record in dataContext.Cards.MainTypes.ConfigureAwait(false))
                hashSet.Add(record.Value);
            return hashSet.ToImmutableHashSet();
        }

        private async Task<IReadOnlyCollection<string>> LoadSubTypes()
        {
            var hashSet = new HashSet<string>();
            await foreach (var record in dataContext.Cards.SubTypes.ConfigureAwait(false))
                hashSet.Add(record.Value);
            return hashSet.ToImmutableHashSet();
        }

        private async Task<IEnumerable<(string, int)>> LoadNameIdPairsAsync()
        {
            var nameIdPairs = new HashSet<(string, int)>();

            // Need to do a seperate search for 'Conspiracy' type as they do not show unless specified.
            await ProcessUrl($@"http://gatherer.wizards.com/Pages/Search/Default.aspx?output=compact&set=[""{setName}""]&type=[""Conspiracy""]&sort=cn+")
                .ConfigureAwait(false);
            await ProcessUrl($@"http://gatherer.wizards.com/Pages/Search/Default.aspx?output=compact&set=[""{setName}""]&sort=cn+")
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

                static IEnumerable<(string, int)> GetNameIdPairs(HtmlNode htmlNode)
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

        private async Task<(string Name, int MultiverseId)> FindClosestNameIdPair(string name, int multiverseId)
        {
            var nameIdPairs = await NameIdPairs.ConfigureAwait(false);
            var matchingPairs = nameIdPairs
                .Where(nameIdPair => nameIdPair.Name == name);
            var closestPair = matchingPairs.First();
            var minAbsDifference = Math.Abs(matchingPairs.First().MultiverseId - multiverseId);
            foreach (var pair in matchingPairs)
            {
                var absDifference = Math.Abs(pair.MultiverseId - multiverseId);
                if (absDifference < minAbsDifference)
                {
                    closestPair = pair;
                    minAbsDifference = absDifference;
                }
            }
            return closestPair;
        }

        private static Task<HtmlNode> GetHtmlNodeAsync(string url) 
            => GetHtmlNodeAsync(url, Encoding.UTF8);

        private static async Task<HtmlNode> GetHtmlNodeAsync(string url, Encoding encoding)
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
                    await Task.Delay(5000)
                        .ConfigureAwait(false);
                    continue;
                }
            throw new AggregateException(exceptions);
        }

        private void ReportProgress(int current, int total, string? detail = null)
            => progress.Report(Progress.Create($"Loading card data: {setName}", current, total, detail));

        [Flags]
        private enum ColorIdentity
        {
            Colorless   = 0x00,
            White       = 0x01,
            Blue        = 0x02,
            Black       = 0x04,
            Red         = 0x08,
            Green       = 0x10
        }
    }
}
