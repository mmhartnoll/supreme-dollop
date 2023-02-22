using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using MindSculptor.Tools.Applications.Harvester.Extensions;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.Applications.Harvester.Processing.Gatherer
{
    internal class GathererMultiFaceCardAdapter : GathererCardAdapter
    {
        private readonly Lazy<IEnumerable<string>> validSelectorPrefixLoader;
        private readonly Lazy<string> linkedFaceNameLoader;

        protected IEnumerable<string> ValidSelectorPrefixes => validSelectorPrefixLoader.Value;

        public string LinkedFaceName => linkedFaceNameLoader.Value;

        protected GathererMultiFaceCardAdapter(HtmlNode htmlNode, string name, int multiverseId)
            : base(htmlNode, name, multiverseId)
        {
            validSelectorPrefixLoader = new Lazy<IEnumerable<string>>(LoadValidSelectorPrefixes);
            linkedFaceNameLoader = new Lazy<string>(LoadLinkedFaceName);
        }

        public static GathererMultiFaceCardAdapter Load(HtmlNode htmlNode, string name, int multiverseId)
            => new GathererMultiFaceCardAdapter(htmlNode, name, multiverseId);

        protected override bool LoadIsPrimaryFaceValue()
            => ValidSelectorPrefixes.First() == SelectorPrefix;

        protected override string LoadSelectorPrefix()
        {
            foreach (var selectorPrefix in ValidSelectorPrefixes)
            {
                var verificationName = HtmlNode
                    .QuerySelector($"{selectorPrefix}_nameRow .value")?
                    .InnerHtml?
                    .RemoveLineBreaks();
                if (verificationName != null && verificationName == Name)
                    return selectorPrefix;
            }
            throw new Exception($"Failed to find an appropriate selector prefix for card '{Name}'.");
        }

        private IEnumerable<string> LoadValidSelectorPrefixes()
        {
            foreach (var extension in SelectorPrefixExtensions)
            {
                var selectorPrefix = SelectorPrefixBase + extension;
                var verificationName = HtmlNode
                    .QuerySelector($"{selectorPrefix}_nameRow .value")?
                    .InnerHtml?
                    .RemoveLineBreaks();
                if (verificationName != null)
                    yield return selectorPrefix;
            }
        }

        private string LoadLinkedFaceName()
        {
            var selector = ValidSelectorPrefixes.First(selectorPrefix => selectorPrefix != SelectorPrefix);
            return HtmlNode
                .QuerySelector($"{selector}_nameRow .value")
                .InnerHtml
                .RemoveLineBreaks();
        }

        protected static readonly string[] SelectorPrefixExtensions = new[]
        {
            "_ctl02",
            "_ctl03",
            "_ctl04"
        };
    }
}
