using MindSculptor.App.MtgaOverlay.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class MatchCardFaceView
    {
        public string Name { get; }
        public IEnumerable<ManaCost> CastingCost { get; }
        public IEnumerable<string> MainTypes { get; }

        private MatchCardFaceView(string name, IEnumerable<ManaCost> castingCost, IEnumerable<string> mainTypes)
        {
            Name        = name;
            CastingCost = castingCost;
            MainTypes   = mainTypes;
        }

        public static async Task<MatchCardFaceView> LoadAsync(CardFaceModel model)
        {
            var castingCost = await model.GetCastingCostAsync().ConfigureAwait(false);
            var mainTypes   = await model.GetMainTypesAsync().ConfigureAwait(false);
            return new MatchCardFaceView(model.Name, castingCost, mainTypes);
        }
    }
}
