using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class CardFace
    {
        private readonly Lazy<ColorIdentity> colorIdentityLoader;

        public string Name { get; }
        public IEnumerable<ManaCost> CastingCost { get; }
        public IEnumerable<string> SuperTypes { get; }
        public IEnumerable<string> MainTypes { get; }
        public IEnumerable<string> SubTypes { get; }

        public ColorIdentity ColorIdentity => colorIdentityLoader.Value;

        public CardFace(string name, IEnumerable<ManaCost> castingCost, IEnumerable<string> superTypes, IEnumerable<string> mainTypes, IEnumerable<string> subTypes)
        {
            Name        = name;
            CastingCost = castingCost;
            SuperTypes  = superTypes;
            MainTypes   = mainTypes;
            SubTypes    = subTypes;

            colorIdentityLoader = new Lazy<ColorIdentity>(LoadColorIdentity);
        }

        private ColorIdentity LoadColorIdentity()
            => CastingCost.Select(manaSymbol => manaSymbol.ColorIdentity).Aggregate(ColorIdentity.Colorless, (a, b) => a |= b);
    }
}
