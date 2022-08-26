using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class CardFaceModel : Model
    {
        private readonly FaceRecord faceRecord;

        private readonly Lazy<Task<IEnumerable<ManaCost>>> castingCostLoader;
        private readonly Lazy<Task<IEnumerable<string>>>   superTypesLoader;
        private readonly Lazy<Task<IEnumerable<string>>>   mainTypesLoader;
        private readonly Lazy<Task<IEnumerable<string>>>   subTypesLoader;
        private readonly Lazy<Task<ColorIdentity>>         colorIdentityLoader;

        public string Name => faceRecord.Name;

        public CardFaceModel(DataContext dataContext, FaceRecord faceRecord) 
            : base(dataContext)
        {
            this.faceRecord = faceRecord;

            castingCostLoader   = new Lazy<Task<IEnumerable<ManaCost>>> (LoadCastingCost);
            superTypesLoader    = new Lazy<Task<IEnumerable<string>>>   (LoadSuperTypesAsync);
            mainTypesLoader     = new Lazy<Task<IEnumerable<string>>>   (LoadMainTypesAsync);
            subTypesLoader      = new Lazy<Task<IEnumerable<string>>>   (LoadSubTypesAsync);
            colorIdentityLoader = new Lazy<Task<ColorIdentity>>         (LoadColorIdentityAsync);
        }

        private async Task<IEnumerable<ManaCost>> LoadCastingCost()
        {
            var castingCost = new List<ManaCost>();
            var faceHasCastingCostRecords = await DataContext.Cards.FaceHasCastingCost
                .QueryWhere(record => record.FaceRecord == faceRecord)
                .OrderBy(record => record.Ordinal)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var faceHasCastingCostRecord in faceHasCastingCostRecords)
            {
                var manaSymbolRecord = await DataContext.Cards.ManaSymbols
                    .QueryWhere(record => record.Id == faceHasCastingCostRecord.ManaSymbolId)
                    .SingleAsync()
                    .ConfigureAwait(false);

                var colorIdentity = ColorIdentity.Colorless;
                colorIdentity |= manaSymbolRecord.HasWhiteIdentity ? ColorIdentity.White : 0;
                colorIdentity |= manaSymbolRecord.HasBlueIdentity  ? ColorIdentity.Blue  : 0;
                colorIdentity |= manaSymbolRecord.HasBlackIdentity ? ColorIdentity.Black : 0;
                colorIdentity |= manaSymbolRecord.HasRedIdentity   ? ColorIdentity.Red   : 0;
                colorIdentity |= manaSymbolRecord.HasGreenIdentity ? ColorIdentity.Green : 0;

                castingCost.Add(new ManaCost(manaSymbolRecord.Code, faceHasCastingCostRecord.Count, colorIdentity));
            }
            return castingCost.Enumerate();
        }

        private async Task<IEnumerable<string>> LoadSuperTypesAsync()
        {
            var superTypes = new List<string>();
            var faceHasSuperTypeRecords = await DataContext.Cards.FaceHasSuperTypes
                .QueryWhere(record => record.FaceRecord == faceRecord)
                .OrderBy(record => record.Ordinal)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var faceHasSuperTypeRecord in faceHasSuperTypeRecords)
            {
                var superTypeRecord = await DataContext.Cards.SuperTypes
                    .QueryWhere(record => record.Id == faceHasSuperTypeRecord.SuperTypeId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                superTypes.Add(superTypeRecord.Value);
            }
            return superTypes.Enumerate();
        }

        private async Task<IEnumerable<string>> LoadMainTypesAsync()
        {
            var mainTypes = new List<string>();
            var faceHasMainTypeRecords = await DataContext.Cards.FaceHasMainTypes
                .QueryWhere(record => record.FaceRecord == faceRecord)
                .OrderBy(record => record.Ordinal)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var faceHasMainTypeRecord in faceHasMainTypeRecords)
            {
                var mainTypeRecord = await DataContext.Cards.MainTypes
                    .QueryWhere(record => record.Id == faceHasMainTypeRecord.MainTypeId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                mainTypes.Add(mainTypeRecord.Value);
            }
            return mainTypes.Enumerate();
        }

        private async Task<IEnumerable<string>> LoadSubTypesAsync()
        {
            var subTypes = new List<string>();
            var faceHasSubTypeRecords = await DataContext.Cards.FaceHasSubTypes
                .QueryWhere(record => record.FaceRecord == faceRecord)
                .OrderBy(record => record.Ordinal)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var faceHasSubTypeRecord in faceHasSubTypeRecords)
            {
                var subTypeRecord = await DataContext.Cards.SubTypes
                    .QueryWhere(record => record.Id == faceHasSubTypeRecord.SubTypeId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                subTypes.Add(subTypeRecord.Value);
            }
            return subTypes.Enumerate();
        }

        private async Task<ColorIdentity> LoadColorIdentityAsync()
        {
            var castingCost = await GetCastingCostAsync().ConfigureAwait(false);
            return castingCost.Select(manaCost => manaCost.ColorIdentity).Aggregate(ColorIdentity.Colorless, (accum, manaCost) => accum |= manaCost);
        }

        public async Task<IEnumerable<ManaCost>> GetCastingCostAsync()
            => await castingCostLoader.Value.ConfigureAwait(false);

        public async Task<IEnumerable<string>> GetSuperTypesAsync()
            => await superTypesLoader.Value.ConfigureAwait(false);

        public async Task<IEnumerable<string>> GetMainTypesAsync()
            => await mainTypesLoader.Value.ConfigureAwait(false);

        public async Task<IEnumerable<string>> GetSubTypesAsync()
            => await subTypesLoader.Value.ConfigureAwait(false);

        public async Task<ColorIdentity> GetColorIdentityAsync()
            => await colorIdentityLoader.Value.ConfigureAwait(false);
    }
}
