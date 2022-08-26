using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.DataAccess.Context.Query;
using MindSculptor.Tools.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.Tools.Data
{
    public class CardDataProcessor
    {
        private readonly DataContext dataContext;

        private readonly AsyncCachedLookup<string, CardTypeRecord> cardTypeRecordLookup;

        private readonly AsyncCachedLookup<string, SuperTypeRecord> superTypeRecordLookup;
        private readonly AsyncCachedLookup<string, MainTypeRecord> mainTypeRecordLookup;
        private readonly AsyncCachedLookup<string, SubTypeRecord> subTypeRecordLookup;
        private readonly AsyncCachedLookup<string, ManaSymbolRecord> manaSymbolRecordLookup;

        private readonly AsyncCachedLookup<string, RarityTypeRecord> rarityTypeRecordLookup;
        private readonly AsyncCachedLookup<string, SubsetTypeRecord> subsetTypeRecordLookup;
        private readonly AsyncCachedLookup<string, PrintingTypeRecord> printingTypeRecordLookup;

        public CardDataProcessor(DataContext dataContext)
        {
            this.dataContext = dataContext;

            cardTypeRecordLookup = new AsyncCachedLookup<string, CardTypeRecord>(key => dataContext.Cards.CardTypes.QueryWhere(record => record.Value == key).TryGetSingleAsync());

            superTypeRecordLookup = new AsyncCachedLookup<string, SuperTypeRecord>(key => dataContext.Cards.SuperTypes.QueryWhere(record => record.Value == key).TryGetSingleAsync());
            mainTypeRecordLookup = new AsyncCachedLookup<string, MainTypeRecord>(key => dataContext.Cards.MainTypes.QueryWhere(record => record.Value == key).TryGetSingleAsync());
            subTypeRecordLookup = new AsyncCachedLookup<string, SubTypeRecord>(key => dataContext.Cards.SubTypes.QueryWhere(record => record.Value == key).TryGetSingleAsync());
            manaSymbolRecordLookup = new AsyncCachedLookup<string, ManaSymbolRecord>(key => dataContext.Cards.ManaSymbols.QueryWhere(record => record.Code == key).TryGetSingleAsync());

            rarityTypeRecordLookup = new AsyncCachedLookup<string, RarityTypeRecord>(key => dataContext.Cards.RarityTypes.QueryWhere(rarityType => rarityType.Value == key).TryGetSingleAsync());
            subsetTypeRecordLookup = new AsyncCachedLookup<string, SubsetTypeRecord>(key => dataContext.Cards.SubsetTypes.QueryWhere(subsetType => subsetType.Value == key).TryGetSingleAsync());
            printingTypeRecordLookup = new AsyncCachedLookup<string, PrintingTypeRecord>(key => dataContext.Cards.PrintingTypes.QueryWhere(printingType => printingType.Value == key).TryGetSingleAsync());
        }

        public async Task ProcessCard(Card card)
        {
            BaseRecord baseRecord;
            FaceRecord primaryFaceRecord;
            var primaryFaceResult = await dataContext.Cards.Faces
                .QueryWhere(record => record.Name == card.PrimaryCardFace.Name)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (primaryFaceResult.Success)
            {
                primaryFaceRecord = primaryFaceResult.Value;
                baseRecord = await dataContext.Cards.Bases
                    .QueryWhere(record => record.Id == primaryFaceRecord.BaseId)
                    .SingleAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                var cardTypeRecord = await cardTypeRecordLookup.GetValueAsync(card.CardType).ConfigureAwait(false);
                baseRecord = await dataContext.Cards.Bases
                    .NewRecordAsync(cardTypeRecord)
                    .ConfigureAwait(false);
                primaryFaceRecord = await dataContext.Cards.Faces
                    .NewRecordAsync(baseRecord, card.PrimaryCardFace.Name, true);
            }

            await ProcessCardFaceDetails(card.PrimaryCardFace, primaryFaceRecord).ConfigureAwait(false);

            var setCodeExtension = card.HasSetCodeExtension ?
                card.SetCodeExtension : null;
            var setRecord = await dataContext.Cards.Sets
                .QueryWhere(record => 
                    record.Code == card.SetCode &&
                    record.CodeExtension == setCodeExtension)
                .SingleAsync()
                .ConfigureAwait(false);

            var collectorsNumber = card.HasCollectorsNumber ?
                card.CollectorsNumber : (int?)null;
            var subsetTypeRecord = await subsetTypeRecordLookup.GetValueAsync(card.SubsetType).ConfigureAwait(false);

            SetInclusionRecord setInclusionRecord;
            var setInclusionResult = await dataContext.Cards.SetInclusions
                .QueryWhere(record =>
                    record.SetRecord == setRecord &&
                    record.SubsetTypeRecord == subsetTypeRecord &&
                    record.BaseRecord == baseRecord &&
                    record.CollectorsNumber == collectorsNumber)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (setInclusionResult.Success)
                setInclusionRecord = setInclusionResult.Value;
            else
            {
                int logicalOrdinal;
                var previousSetInclusionResult = await dataContext.Cards.SetInclusions
                    .QueryWhere(record =>
                        record.SetRecord == setRecord &&
                        record.SubsetTypeRecord == subsetTypeRecord)
                    .OrderBy(record => record.LogicalOrdinal, SortDirection.Descending)
                    .TryGetFirstAsync()
                    .ConfigureAwait(false);
                if (previousSetInclusionResult.Success)
                    logicalOrdinal = previousSetInclusionResult.Value.LogicalOrdinal + 1;
                else
                    logicalOrdinal = 1;

                var rarityTypeRecord = await rarityTypeRecordLookup.GetValueAsync(card.Rarity).ConfigureAwait(false);
                setInclusionRecord = await dataContext.Cards.SetInclusions
                    .NewRecordAsync(setRecord, subsetTypeRecord, baseRecord, rarityTypeRecord, logicalOrdinal, collectorsNumber)
                    .ConfigureAwait(false);
            }

            BasePrintingRecord basePrintingRecord;
            var printingTypeRecord = await printingTypeRecordLookup.GetValueAsync(card.PrintingType).ConfigureAwait(false);
            var basePrintingResult = await dataContext.Cards.BasePrintings
                .QueryWhere(record => 
                    record.SetInclusionRecord == setInclusionRecord &&
                    record.PrintingTypeRecord == printingTypeRecord)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (basePrintingResult.Success)
                basePrintingRecord = basePrintingResult.Value;
            else
            {
                ArtistRecord artistRecord;
                var artistResult = await dataContext.Cards.Artists
                    .QueryWhere(record => record.Name == card.Artist)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (artistResult.Success)
                    artistRecord = artistResult.Value;
                else
                    artistRecord = await dataContext.Cards.Artists
                        .NewRecordAsync(card.Artist)
                        .ConfigureAwait(false);
                basePrintingRecord = await dataContext.Cards.BasePrintings
                    .NewRecordAsync(setInclusionRecord, printingTypeRecord, artistRecord)
                    .ConfigureAwait(false);
            }

            FacePrintingRecord primaryFacePrintingRecord;
            var primaryFacePrintingResult = await dataContext.Cards.FacePrintings
                .QueryWhere(record =>
                    record.BasePrintingRecord == basePrintingRecord &&
                    record.FaceRecord == primaryFaceRecord)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (primaryFacePrintingResult.Success)
                primaryFacePrintingRecord = primaryFacePrintingResult.Value;
            else
                primaryFacePrintingRecord = await dataContext.Cards.FacePrintings
                    .NewRecordAsync(basePrintingRecord, primaryFaceRecord)
                    .ConfigureAwait(false);

            if (card.PrimaryCardFace.HasFlavorText)
            {
                var flavorTextResult = await dataContext.Cards.FlavorText
                    .QueryWhere(record => record.FacePrintingRecord == primaryFacePrintingRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (flavorTextResult.Success)
                {
                    var flavorTextRecord = flavorTextResult.Value;
                    flavorTextRecord.Value = card.PrimaryCardFace.FlavorText;
                    await flavorTextRecord.UpdateRecordAsync().ConfigureAwait(false);
                }
                else
                    await dataContext.Cards.FlavorText.NewRecordAsync(primaryFacePrintingRecord, card.PrimaryCardFace.FlavorText).ConfigureAwait(false);
            }

            if (card.HasSecondaryCardFace)
            {
                FaceRecord secondaryFaceRecord;
                var secondaryFaceResult = await dataContext.Cards.Faces
                    .QueryWhere(record => record.Name == card.SecondaryCardFace.Name)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (secondaryFaceResult.Success)
                    secondaryFaceRecord = secondaryFaceResult.Value;
                else
                    secondaryFaceRecord = await dataContext.Cards.Faces
                        .NewRecordAsync(baseRecord, card.SecondaryCardFace.Name, false)
                        .ConfigureAwait(false);

                await ProcessCardFaceDetails(card.SecondaryCardFace, secondaryFaceRecord).ConfigureAwait(false);

                FacePrintingRecord secondaryFacePrintingRecord;
                var secondaryFacePrintingResult = await dataContext.Cards.FacePrintings
                    .QueryWhere(record =>
                        record.BasePrintingRecord == basePrintingRecord &&
                        record.FaceRecord == secondaryFaceRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (secondaryFacePrintingResult.Success)
                    secondaryFacePrintingRecord = secondaryFacePrintingResult.Value;
                else
                    secondaryFacePrintingRecord = await dataContext.Cards.FacePrintings
                        .NewRecordAsync(basePrintingRecord, secondaryFaceRecord)
                        .ConfigureAwait(false);

                if (card.SecondaryCardFace.HasFlavorText)
                {
                    var flavorTextResult = await dataContext.Cards.FlavorText
                        .QueryWhere(record => record.FacePrintingRecord == secondaryFacePrintingRecord)
                        .TryGetSingleAsync()
                        .ConfigureAwait(false);
                    if (flavorTextResult.Success)
                    {
                        var flavorTextRecord = flavorTextResult.Value;
                        flavorTextRecord.Value = card.SecondaryCardFace.FlavorText;
                        await flavorTextRecord.UpdateRecordAsync().ConfigureAwait(false);
                    }
                    else
                        await dataContext.Cards.FlavorText.NewRecordAsync(secondaryFacePrintingRecord, card.SecondaryCardFace.FlavorText).ConfigureAwait(false);
                }
            }

            if (card.HasMtgaCardId)
            {
                var digitalCardResult = await dataContext.Mtga.DigitalCards
                    .QueryWhere(record => record.MtgaCardId == card.MtgaCardId)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (!digitalCardResult.Success)
                    await dataContext.Mtga.DigitalCards.NewRecordAsync(basePrintingRecord, card.MtgaCardId).ConfigureAwait(false);
            }
        }

        private async Task ProcessCardFaceDetails(CardFace cardFace, FaceRecord faceRecord)
        {
            await ProcessCardFaceCastingCost(cardFace, faceRecord).ConfigureAwait(false);
            await ProcessCardFaceTypes(cardFace, faceRecord).ConfigureAwait(false);

            if (cardFace.HasOracleText)
            {
                var oracleTextResult = await dataContext.Cards.OracleText
                    .QueryWhere(record => record.FaceRecord == faceRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (oracleTextResult.Success)
                {
                    var oracleTextRecord = oracleTextResult.Value;
                    oracleTextRecord.Value = cardFace.OracleText;
                    await oracleTextRecord.UpdateRecordAsync().ConfigureAwait(false);
                }
                else
                    await dataContext.Cards.OracleText.NewRecordAsync(faceRecord, cardFace.OracleText).ConfigureAwait(false);
            }
            if (cardFace.HasPowerAndToughness)
            {
                var powerToughnessResult = await dataContext.Cards.PowerToughness
                    .QueryWhere(record => record.FaceRecord == faceRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (!powerToughnessResult.Success)
                    await dataContext.Cards.PowerToughness
                        .NewRecordAsync(faceRecord, cardFace.Power.Value, cardFace.Power.Format, cardFace.Toughness.Value, cardFace.Toughness.Format)
                        .ConfigureAwait(false);
            }
            if (cardFace.HasLoyalty)
            {
                var loyaltyResult = await dataContext.Cards.Loyalty
                    .QueryWhere(record => record.FaceRecord == faceRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (!loyaltyResult.Success)
                    await dataContext.Cards.Loyalty.NewRecordAsync(faceRecord, cardFace.Loyalty.Value, cardFace.Loyalty.Format).ConfigureAwait(false);
            }
        }

        private async Task ProcessCardFaceCastingCost(CardFace cardFace, FaceRecord faceRecord)
        {
            var manaSymbolRecords = await cardFace.CastingCost
                .SelectAsync(async manaSymbol => await manaSymbolRecordLookup.TryGetValueAsync(manaSymbol.SymbolType).ConfigureAwait(false))
                .WhereAsync(result => result.Success)
                .SelectAsync(result => result.Value)
                .ToListAsync()
                .ConfigureAwait(false);
            var faceHasCastingCostRecords = await dataContext.Cards.FaceHasCastingCost
                .QueryWhere(record => record.FaceRecord == faceRecord)
                .OrderBy(record => record.Ordinal)
                .ToListAsync()
                .ConfigureAwait(false);

            var missingManaSymbols = cardFace.CastingCost.Select(manaCost => manaCost.SymbolType)
                .Except(manaSymbolRecords.Select(record => record.Code));
            if (missingManaSymbols.Any())
            {
                var formattedManaSymbols = string.Join(", ", missingManaSymbols.Select(manaSymbol => $"'{manaSymbol}'"));
                var s = missingManaSymbols.Count() > 1 ? "s" : string.Empty;
                throw new InvalidOperationException($"The mana symbol{s} {formattedManaSymbols} could not be found in the database. You may need to add the records manually.");
            }

            var castingCostMatch = manaSymbolRecords.Zip(cardFace.CastingCost, (record, manaCost) => (record.Id, manaCost.Count))
                .SequenceEqual(faceHasCastingCostRecords.Select(record => (record.ManaSymbolId, record.Count)));
            if (!castingCostMatch)
            {
                int ordinal = 1;
                foreach (var record in faceHasCastingCostRecords)
                    await record.DeleteRecordAsync().ConfigureAwait(false);
                foreach (var manaCost in cardFace.CastingCost)
                {
                    var manaSymbolRecord = manaSymbolRecords.Where(record => record.Code == manaCost.SymbolType).Single();
                    await dataContext.Cards.FaceHasCastingCost
                        .NewRecordAsync(faceRecord, manaSymbolRecord, ordinal++, manaCost.Count)
                        .ConfigureAwait(false);
                }
            }
        }

        private async Task ProcessCardFaceTypes(CardFace cardFace, FaceRecord faceRecord)
        {
            var superTypeRecords = await cardFace.SuperTypes
                .SelectAsync(async type => await superTypeRecordLookup.TryGetValueAsync(type).ConfigureAwait(false))
                .WhereAsync(result => result.Success)
                .SelectAsync(result => result.Value)
                .ToListAsync()
                .ConfigureAwait(false);
            var mainTypeRecords = await cardFace.MainTypes
                .SelectAsync(async type => await mainTypeRecordLookup.TryGetValueAsync(type).ConfigureAwait(false))
                .WhereAsync(result => result.Success)
                .SelectAsync(result => result.Value)
                .ToListAsync()
                .ConfigureAwait(false);
            var subTypeRecords = await cardFace.SubTypes
                .SelectAsync(async type => await subTypeRecordLookup.TryGetValueAsync(type).ConfigureAwait(false))
                .WhereAsync(result => result.Success)
                .SelectAsync(result => result.Value)
                .ToListAsync()
                .ConfigureAwait(false);

            var requiredTypes = cardFace.SuperTypes
                .Union(cardFace.MainTypes)
                .Union(cardFace.SubTypes);
            var identifiedTypes = superTypeRecords.Select(record => record.Value)
                .Union(mainTypeRecords.Select(record => record.Value))
                .Union(subTypeRecords.Select(record => record.Value));

            var missingTypes = requiredTypes.Except(identifiedTypes);
            if (missingTypes.Any())
            {
                var formattedMissingTypes = string.Join(", ", missingTypes.Select(type => $"'{type}'"));
                var s = missingTypes.Count() > 1 ? "s" : string.Empty;
                throw new InvalidOperationException($"The type{s} {formattedMissingTypes} could not be found in the database. You may need to add the record{s} manually.");
            }

            var faceHasSuperTypeRecords = await dataContext.Cards.FaceHasSuperTypes
                .QueryWhere(record => record.FaceRecord == faceRecord)
                .OrderBy(record => record.Ordinal)
                .ToListAsync()
                .ConfigureAwait(false);
            var faceHasMainTypeRecords = await dataContext.Cards.FaceHasMainTypes
                .QueryWhere(record => record.FaceRecord == faceRecord)
                .OrderBy(record => record.Ordinal)
                .ToListAsync()
                .ConfigureAwait(false);
            var faceHasSubTypeRecords = await dataContext.Cards.FaceHasSubTypes
                .QueryWhere(record => record.FaceRecord == faceRecord)
                .OrderBy(record => record.Ordinal)
                .ToListAsync()
                .ConfigureAwait(false);

            var superTypesMatch = superTypeRecords.Select(record => record.Id)
                .SequenceEqual(faceHasSuperTypeRecords.Select(record => record.SuperTypeId));
            if (!superTypesMatch)
            {
                var ordinal = 1;
                foreach (var record in faceHasSuperTypeRecords)
                    await record.DeleteRecordAsync().ConfigureAwait(false);
                foreach (var record in superTypeRecords)
                    await dataContext.Cards.FaceHasSuperTypes.NewRecordAsync(faceRecord, record, ordinal++).ConfigureAwait(false);
            }

            var mainTypesMatch = mainTypeRecords.Select(record => record.Id)
                .SequenceEqual(faceHasMainTypeRecords.Select(record => record.MainTypeId));
            if (!mainTypesMatch)
            {
                var ordinal = 1;
                foreach (var record in faceHasMainTypeRecords)
                    await record.DeleteRecordAsync().ConfigureAwait(false);
                foreach (var record in mainTypeRecords)
                    await dataContext.Cards.FaceHasMainTypes.NewRecordAsync(faceRecord, record, ordinal++).ConfigureAwait(false);
            }

            var subTypesMatch = subTypeRecords.Select(record => record.Id)
                .SequenceEqual(faceHasSubTypeRecords.Select(record => record.SubTypeId));
            if (!subTypesMatch)
            {
                var ordinal = 1;
                foreach (var record in faceHasSubTypeRecords)
                    await record.DeleteRecordAsync().ConfigureAwait(false);
                foreach (var record in subTypeRecords)
                    await dataContext.Cards.FaceHasSubTypes.NewRecordAsync(faceRecord, record, ordinal++).ConfigureAwait(false);
            }
        }
    }
}
