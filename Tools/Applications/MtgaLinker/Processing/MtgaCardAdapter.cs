using MindSculptor.Tools.Data;
using MindSculptor.Tools.Exceptions;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Mindsculptor.Tools.Applications.MtgaLinker.Processing
{
    internal class MtgaCardAdapter
    {
        private readonly JsonElement jsonElement;

        private readonly Lazy<string> nameLoader;
        private readonly Lazy<IEnumerable<ManaCost>> castingCostLoader;
        private readonly Lazy<IEnumerable<string>> superTypesLoader;
        private readonly Lazy<IEnumerable<string>> mainTypesLoader;
        private readonly Lazy<IEnumerable<string>> subTypesLoader;
        private readonly Lazy<string?> oracleTextLoader;
        private readonly Lazy<FormattedInteger?> powerLoader;
        private readonly Lazy<FormattedInteger?> toughnessLoader;
        private readonly Lazy<FormattedInteger?> loyaltyLoader;
        private readonly Lazy<string> setCodeLoader;
        private readonly Lazy<int> collectorsNumberLoader;
        private readonly Lazy<string> rarityLoader;
        private readonly Lazy<string?> flavorTextLoader;
        private readonly Lazy<string> artistLoader;
        private readonly Lazy<int> mtgaCardIdLoader;

        public bool HasOracleText => oracleTextLoader.Value != null;
        public bool HasPowerAndToughness => powerLoader.Value != null && toughnessLoader.Value != null;
        public bool HasLoyalty => loyaltyLoader.Value != null;
        public bool HasFlavorText => flavorTextLoader.Value != null;

        public string Name => nameLoader.Value;
        public IEnumerable<ManaCost> CastingCost => castingCostLoader.Value;
        public IEnumerable<string> SuperTypes => superTypesLoader.Value;
        public IEnumerable<string> MainTypes => mainTypesLoader.Value;
        public IEnumerable<string> SubTypes => subTypesLoader.Value;
        public string OracleText => oracleTextLoader.Value ?? throw new PropertyUndefinedException(nameof(OracleText), nameof(HasOracleText));
        public FormattedInteger Power => powerLoader.Value ?? throw new PropertyUndefinedException(nameof(Power), nameof(HasPowerAndToughness));
        public FormattedInteger Toughness => toughnessLoader.Value ?? throw new PropertyUndefinedException(nameof(Toughness), nameof(HasPowerAndToughness));
        public FormattedInteger Loyalty => loyaltyLoader.Value ?? throw new PropertyUndefinedException(nameof(Loyalty), nameof(HasLoyalty));
        public string FlavorText => flavorTextLoader.Value ?? throw new PropertyUndefinedException(nameof(FlavorText), nameof(HasFlavorText));

        public string SetCode => setCodeLoader.Value;
        public int CollectorsNumber => collectorsNumberLoader.Value;
        public string Rarity => rarityLoader.Value;
        public string Artist => artistLoader.Value;
        public int MtgaCardId => mtgaCardIdLoader.Value;

        private MtgaCardAdapter(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;

            nameLoader              = new Lazy<string>                  (LoadName);
            castingCostLoader       = new Lazy<IEnumerable<ManaCost>>   (LoadCastingCost);
            superTypesLoader        = new Lazy<IEnumerable<string>>     (LoadSuperTypes);
            mainTypesLoader         = new Lazy<IEnumerable<string>>     (LoadMainTypes);
            subTypesLoader          = new Lazy<IEnumerable<string>>     (LoadSubTypes);
            oracleTextLoader        = new Lazy<string?>                 (LoadOracleText);
            powerLoader             = new Lazy<FormattedInteger?>       (LoadPower);
            toughnessLoader         = new Lazy<FormattedInteger?>       (LoadToughness);
            loyaltyLoader           = new Lazy<FormattedInteger?>       (LoadLoyalty);
            flavorTextLoader        = new Lazy<string?>                 (LoadFlavorText);
            setCodeLoader           = new Lazy<string>                  (LoadSetCode);
            collectorsNumberLoader  = new Lazy<int>                     (LoadCollectorsNumber);
            rarityLoader            = new Lazy<string>                  (LoadRarity);
            artistLoader            = new Lazy<string>                  (LoadArtist);
            mtgaCardIdLoader        = new Lazy<int>                     (LoadMtgaCardId);
        }

        public static bool TryGetAdapter(JsonElement jsonElement, out MtgaCardAdapter result)
        {
            result = default!;
            if (jsonElement.GetProperty("isToken").GetBoolean())
                return false;
            if (jsonElement.GetProperty("set").GetString() == "ArenaSUP")
                return false;

            result = new MtgaCardAdapter(jsonElement);
            return true;
        }

        private string LoadName()
        {
            var titleId = jsonElement.GetProperty("titleId").GetInt32();
            return LocaleStringsLookup.GetValue(titleId);
        }

        private IEnumerable<ManaCost> LoadCastingCost()
        {
            if (MainTypes.Contains("Land"))
                return Enumerable.Empty<ManaCost>();

            var castingCost = new List<ManaCost>();
            var castingCostString = jsonElement.GetProperty("castingcost").GetString();
            var manaSymbolParts = castingCostString.Split('o', StringSplitOptions.RemoveEmptyEntries);
            foreach (var uniqueManaSymbol in manaSymbolParts.Distinct())
            {
                if (int.TryParse(uniqueManaSymbol, out var value))
                    castingCost.Add(new ManaCost(1.ToString(), value));
                else
                {
                    var symbolCount = manaSymbolParts.Where(symbol => symbol == uniqueManaSymbol).Count();
                    var symbolType = uniqueManaSymbol.Replace("(", null).Replace(")", null).Replace(@"/", null);
                    castingCost.Add(new ManaCost(symbolType, symbolCount));
                }
            }
            return castingCost.Enumerate();
        }

        private IEnumerable<string> LoadSuperTypes()
            => jsonElement.GetProperty("supertypes").EnumerateArray()
                .Select(element => element.GetInt32())
                .Select(id => EnumsLookup.GetValue("SuperType", id))
                .ToList()
                .Enumerate();

        private IEnumerable<string> LoadMainTypes()
            => jsonElement.GetProperty("types").EnumerateArray()
                .Select(element => element.GetInt32())
                .Select(id => EnumsLookup.GetValue("CardType", id))
                .ToList()
                .Enumerate();

        private IEnumerable<string> LoadSubTypes()
            => jsonElement.GetProperty("subtypes").EnumerateArray()
                .Select(element => element.GetInt32())
                .Select(id => EnumsLookup.GetValue("SubType", id))
                .ToList()
                .Enumerate();

        private string? LoadOracleText()
        {
            var oracleText = new StringBuilder();
            var abilitiesElement = jsonElement.GetProperty("abilities");
            foreach (var abilityElement in abilitiesElement.EnumerateArray())
            {
                var textId = abilityElement.GetProperty("textId").GetInt32();
                var abilityText = LocaleStringsLookup.GetValue(textId);
                oracleText.Append($"{abilityText}\n");
            }
            if (oracleText.Length == 0)
                return null;
            oracleText.Remove(oracleText.Length - 1, 1);
            return oracleText.ToString();
        }

        private FormattedInteger? LoadPower()
        {
            if (!MainTypes.Contains("Creature") && !MainTypes.Contains("Vehicle"))
                return null;
            var rawPower = jsonElement.GetProperty("power").GetString();
            var basePowerRegex = Regex.Match(rawPower, @"(\d+)");
            var basePower = basePowerRegex.Success
                ? Convert.ToInt32(basePowerRegex.Groups[1].Value)
                : 0;
            var powerFormat = Regex.Replace(rawPower, @"(\d+)", "$");
            return FormattedInteger.CreateFormatted(basePower, powerFormat);
        }

        private FormattedInteger? LoadToughness()
        {
            if (!MainTypes.Contains("Creature") && !MainTypes.Contains("Vehicle"))
                return null;
            var rawToughness = jsonElement.GetProperty("toughness").GetString();
            var baseToughnessRegex = Regex.Match(rawToughness, @"(\d+)");
            var baseToughness = baseToughnessRegex.Success
                ? Convert.ToInt32(baseToughnessRegex.Groups[1].Value)
                : 0;
            var toughnessFormat = Regex.Replace(rawToughness, @"(\d+)", "$");
            return FormattedInteger.CreateFormatted(baseToughness, toughnessFormat);
        }

        private FormattedInteger? LoadLoyalty()
        {
            if (!MainTypes.Contains("Planeswalker"))
                return null;
            var rawToughness = jsonElement.GetProperty("toughness").GetString();
            var baseToughnessRegex = Regex.Match(rawToughness, @"(\d+)");
            var baseToughness = baseToughnessRegex.Success
                ? Convert.ToInt32(baseToughnessRegex.Groups[1].Value)
                : 0;
            var toughnessFormat = Regex.Replace(rawToughness, @"(\d+)", "$");
            return FormattedInteger.CreateFormatted(baseToughness, toughnessFormat);
        }

        private string? LoadFlavorText()
        {
            var flavorId = jsonElement.GetProperty("flavorId").GetInt32();
            var flavorText = LocaleStringsLookup.GetValue(flavorId);
            if (flavorText.Length == 0)
                return null;
            return flavorText;
        }

        private string LoadSetCode()
        {
            var setCode = jsonElement.GetProperty("set").GetString();

            // MTGA uses code DAR for 'Dominaria'??? Adjust to DOM to match paper.
            if (setCode == "DAR")
                return "DOM";

            return setCode;
        }

        private int LoadCollectorsNumber()
        {
            var collectorsNumberString = jsonElement.GetProperty("CollectorNumber").GetString();
            return Convert.ToInt32(Regex.Replace(collectorsNumberString, @"[^\d]*(\d+)[^\d]*", @"$1"));
        }

        private string LoadRarity()
            => jsonElement.GetProperty("rarity").GetInt32() switch
            {
                1 => "Basic Land",
                2 => "Common",
                3 => "Uncommon",
                4 => "Rare",
                5 => "Mythic Rare",
                _ => throw new InvalidOperationException()
            };

        private string LoadArtist()
            => jsonElement.GetProperty("artistCredit").GetString();

        private int LoadMtgaCardId()
            => jsonElement.GetProperty("grpid").GetInt32();
    }
}
