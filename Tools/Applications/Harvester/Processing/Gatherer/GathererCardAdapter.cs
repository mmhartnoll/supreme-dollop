using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using MindSculptor.Tools.Applications.Harvester.Extensions;
using MindSculptor.Tools.Data;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MindSculptor.Tools.Applications.Harvester.Processing.Gatherer
{
    internal class GathererCardAdapter : HtmlAdapter
    {
        private readonly Lazy<string> selectorPrefixLoader;

        private readonly Lazy<bool> isPrimaryFaceLoader;

        private readonly Lazy<IEnumerable<ManaCost>> castingCostLoader;
        private readonly Lazy<IEnumerable<string>> typesLoader;
        private readonly Lazy<string?> oracleTextLoader;

        private readonly Lazy<FormattedInteger?> powerLoader;
        private readonly Lazy<FormattedInteger?> toughnessLoader;
        private readonly Lazy<FormattedInteger?> loyaltyLoader;

        private readonly Lazy<int?> collectorsNumberLoader;
        private readonly Lazy<string> rarityLoader;
        private readonly Lazy<string?> flavorTextLoader;
        private readonly Lazy<string> artistLoader;


        protected string SelectorPrefix => selectorPrefixLoader.Value;

        public bool HasOracleText => oracleTextLoader.Value != null;
        public bool HasPowerAndToughness => powerLoader.Value != null && toughnessLoader.Value != null;
        public bool HasLoyalty => loyaltyLoader.Value != null;
        public bool HasCollectorsNumber => collectorsNumberLoader.Value != null;
        public bool HasFlavorText => flavorTextLoader.Value != null;

        public bool IsPrimaryFace => isPrimaryFaceLoader.Value;

        public string Name { get; }
        public int MultiverseId { get; }

        public IEnumerable<ManaCost> CastingCost => castingCostLoader.Value;
        public IEnumerable<string> Types => typesLoader.Value;
        public string OracleText => oracleTextLoader.Value ?? throw new InvalidOperationException($"'{nameof(OracleText)}' is not defined. Please check the value of '{nameof(HasOracleText)}' before accessing this property.");

        public FormattedInteger Power => powerLoader.Value ?? throw new InvalidOperationException($"'{nameof(Power)}' is not defined. Please check the value of '{nameof(HasPowerAndToughness)}' before accessing this property.");
        public FormattedInteger Toughness => toughnessLoader.Value ?? throw new InvalidOperationException($"'{nameof(Toughness)}' is not defined. Please check the value of '{nameof(HasPowerAndToughness)}' before accessing this property.");
        public FormattedInteger Loyalty => loyaltyLoader.Value ?? throw new InvalidOperationException($"'{nameof(Loyalty)}' is not defined. Please check the value of '{nameof(HasLoyalty)}' before accessing this property.");

        public int CollectorsNumber => collectorsNumberLoader.Value ?? throw new InvalidOperationException($"'{nameof(CollectorsNumber)}' is not defined. Please check the value of '{nameof(HasCollectorsNumber)}' before accessing this property.");
        public string Rarity => rarityLoader.Value;
        public string FlavorText => flavorTextLoader.Value ?? throw new InvalidOperationException($"'{nameof(FlavorText)}' is not defined. Please check the value of '{nameof(HasFlavorText)}' before accessing this property.");
        public string Artist => artistLoader.Value;

        protected GathererCardAdapter(HtmlNode htmlNode, string name, int multiverseId)
            : base(htmlNode)
        {
            Name = name;
            MultiverseId = multiverseId;

            selectorPrefixLoader = new Lazy<string>(LoadSelectorPrefix);

            isPrimaryFaceLoader = new Lazy<bool>(LoadIsPrimaryFaceValue);

            castingCostLoader = new Lazy<IEnumerable<ManaCost>>(LoadCastingCost);
            typesLoader = new Lazy<IEnumerable<string>>(LoadTypes);
            oracleTextLoader = new Lazy<string?>(LoadOracleText);

            powerLoader = new Lazy<FormattedInteger?>(LoadPower);
            toughnessLoader = new Lazy<FormattedInteger?>(LoadToughness);
            loyaltyLoader = new Lazy<FormattedInteger?>(LoadLoyalty);

            collectorsNumberLoader = new Lazy<int?>(LoadCollectorsNumber);
            rarityLoader = new Lazy<string>(LoadRarity);
            flavorTextLoader = new Lazy<string?>(LoadFlavorText);
            artistLoader = new Lazy<string>(LoadArtist);
        }

        internal static async Task<GathererCardAdapter> LoadAsync(string name, int multiverseId)
        {
            var htmlNode = await LoadHtmlNodeAsync(multiverseId)
                .ConfigureAwait(false);
            var adapter = new GathererCardAdapter(htmlNode, name, multiverseId);

            var verificationName = adapter.HtmlNode
                .QuerySelector($"{adapter.SelectorPrefix}_nameRow .value")?
                .InnerHtml?
                .RemoveLineBreaks();
            if (verificationName != null && verificationName == name)
                return adapter;
            return GathererMultiFaceCardAdapter.Load(htmlNode, name, multiverseId);
        }

        protected static async Task<HtmlNode> LoadHtmlNodeAsync(int multiverseId)
            => await GetHtmlNodeAsync($@"http://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid={multiverseId}")
                .ConfigureAwait(false);

        protected virtual bool LoadIsPrimaryFaceValue() => true;

        private IEnumerable<ManaCost> LoadCastingCost()
        {
            var rawValue = HtmlNode.QuerySelector($"{SelectorPrefix}_manaRow .value")?
                .InnerHtml
                .RemoveLineBreaks()
                .FormatManaSymbols();
            if (rawValue == null)
                yield break;

            var symbolGroups = rawValue
                .Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(symbol => symbol);
            foreach (var symbolGroup in symbolGroups)
                if (int.TryParse(symbolGroup.Key, out var numericKey))
                    yield return new ManaCost("1", numericKey);
                else
                    yield return new ManaCost(symbolGroup.Key, symbolGroup.Count());
        }

        private IEnumerable<string> LoadTypes()
        {
            var rawValue = HtmlNode.QuerySelector($"{SelectorPrefix}_typeRow .value")
                .InnerHtml;
            return rawValue
                .RemoveLineBreaks()
                .Replace("—", string.Empty)
                .Replace("Urza’s", "Urza's")
                .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Enumerate();
        }

        private string? LoadOracleText()
        {
            var rawValue = HtmlNode.QuerySelector($"{SelectorPrefix}_textRow .value")?
                .InnerHtml;
            if (rawValue == null)
                return null;
            return rawValue
                .RemoveLineBreaks()
                .FormatManaSymbols()
                .ConvertTextBlocksToNewLines()
                .RemoveItalicsTags()
                .TrimEnd('\n');
        }

        private FormattedInteger? LoadPower()
        {
            var rawValue = HtmlNode.QuerySelector($"{SelectorPrefix}_ptRow .value")?
                .InnerHtml
                .RemoveLineBreaks();
            if (rawValue == null || !rawValue.Contains('/'))
                return null;

            var rawPower = rawValue.Split('/', StringSplitOptions.RemoveEmptyEntries)
                .First()
                .Trim();

            var basePowerRegex = Regex.Match(rawPower, @"(\d+)");
            var basePower = basePowerRegex.Success
                ? Convert.ToInt32(basePowerRegex.Groups[1].Value)
                : 0;
            var powerFormat = Regex.Replace(rawPower, @"(\d+)", "$");
            return FormattedInteger.CreateFormatted(basePower, powerFormat);
        }

        private FormattedInteger? LoadToughness()
        {
            var rawValue = HtmlNode.QuerySelector($"{SelectorPrefix}_ptRow .value")?
                .InnerHtml
                .RemoveLineBreaks();
            if (rawValue == null || !rawValue.Contains('/'))
                return null;

            var rawToughness = rawValue.Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Last()
                .Trim();

            var baseToughnessRegex = Regex.Match(rawToughness, @"(\d+)");
            var baseToughness = baseToughnessRegex.Success
                ? Convert.ToInt32(baseToughnessRegex.Groups[1].Value)
                : 0;
            var toughnessFormat = Regex.Replace(rawToughness, @"(\d+)", "$");
            return FormattedInteger.CreateFormatted(baseToughness, toughnessFormat);
        }

        private FormattedInteger? LoadLoyalty()
        {
            var rawValue = HtmlNode.QuerySelector($"{SelectorPrefix}_ptRow .value")?
                .InnerHtml
                .RemoveLineBreaks();
            if (rawValue == null || rawValue.Contains('/'))
                return null;

            var baseLoyaltyRegex = Regex.Match(rawValue, @"(\d+)");
            var baseLoyalty = baseLoyaltyRegex.Success
                ? Convert.ToInt32(baseLoyaltyRegex.Groups[1].Value)
                : 0;
            var loyaltyFormat = Regex.Replace(rawValue, @"(\d+)", "$");
            return FormattedInteger.CreateFormatted(baseLoyalty, loyaltyFormat);
        }

        private int? LoadCollectorsNumber()
        {
            var rawValue = HtmlNode.QuerySelector($"{SelectorPrefix}_numberRow .value")?
                .InnerHtml;
            if (rawValue == null)
                return null;
            return Convert.ToInt16(Regex.Replace(
                rawValue.RemoveLineBreaks(), 
                @"(\d+).*", "$1"));
        }

        private string LoadRarity()
            => HtmlNode.QuerySelector($"{SelectorPrefix}_rarityRow .value span")
                .InnerHtml
                .RemoveLineBreaks()
                .Replace("Bonus", "Special");

        private string? LoadFlavorText()
        {
            var rawValue = HtmlNode.QuerySelector($"{SelectorPrefix}_flavorRow .value")?
                .InnerHtml;
            if (rawValue == null)
                return null;
            return rawValue
                .RemoveLineBreaks()
                .FormatManaSymbols()
                .ConvertTextBlocksToNewLines()
                .RemoveItalicsTags()
                .TrimEnd('\n');
        }

        private string LoadArtist()
            => HtmlNode.QuerySelector($"{SelectorPrefix}_artistRow .value a")?
                    .InnerHtml?
                    .RemoveLineBreaks() ??
                "(unknown)";

        protected virtual string LoadSelectorPrefix() => SelectorPrefixBase;

        protected const string SelectorPrefixBase = "#ctl00_ctl00_ctl00_MainContent_SubContent_SubContent";
    }
}
