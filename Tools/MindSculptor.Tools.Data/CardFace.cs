using MindSculptor.Tools.Exceptions;
using MindSculptor.Tools.Extensions;
using System.Collections.Generic;

namespace MindSculptor.Tools.Data
{
    public class CardFace
    {
        private readonly string? oracleText;
        private readonly FormattedInteger? power;
        private readonly FormattedInteger? toughness;
        private readonly FormattedInteger? loyalty;
        private readonly string? flavorText;

        public bool HasOracleText => oracleText != null;
        public bool HasPowerAndToughness => power != null && toughness != null;
        public bool HasLoyalty => loyalty != null;
        public bool HasFlavorText => flavorText != null;

        public string Name { get; }

        public IEnumerable<ManaCost> CastingCost { get; }

        public IEnumerable<string> SuperTypes { get; }
        public IEnumerable<string> MainTypes { get; }
        public IEnumerable<string> SubTypes { get; }

        public string OracleText => oracleText ?? throw new PropertyUndefinedException(nameof(OracleText), nameof(HasOracleText));

        public FormattedInteger Power => power ?? throw new PropertyUndefinedException(nameof(Power), nameof(HasPowerAndToughness));
        public FormattedInteger Toughness => toughness ?? throw new PropertyUndefinedException(nameof(Toughness), nameof(HasPowerAndToughness));
        public FormattedInteger Loyalty => loyalty ?? throw new PropertyUndefinedException(nameof(Loyalty), nameof(HasLoyalty));

        public string FlavorText => flavorText ?? throw new PropertyUndefinedException(nameof(FlavorText), nameof(HasFlavorText));
        

        public CardFace(string name, IEnumerable<ManaCost> castingCost, IEnumerable<string> superTypes, IEnumerable<string> mainTypes, IEnumerable<string> subTypes, 
            string? oracleText, FormattedInteger? power, FormattedInteger? toughness, FormattedInteger? loyalty, string? flavorText)
        {
            Name = name;
            CastingCost             = castingCost.Enumerate();
            SuperTypes              = superTypes.Enumerate();
            MainTypes               = mainTypes.Enumerate();
            SubTypes                = subTypes.Enumerate();
            this.oracleText         = oracleText;
            this.power              = power;
            this.toughness          = toughness;
            this.loyalty            = loyalty;
            this.flavorText         = flavorText;
        }
    }
}
