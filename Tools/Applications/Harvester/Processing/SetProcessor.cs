using MindSculptor.App.AppDataContext;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.DataAccess.Context.Query;
using MindSculptor.Tools.Applications.Harvester.Processing.Gatherer;
using MindSculptor.Tools.Data;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MindSculptor.Tools.Applications.Harvester.Processing
{
    internal class SetProcessor
    {
        private readonly AppDataContext dataContext;
        private readonly SetRecord setRecord;
        private readonly IProgress<Progress> progress;

        private readonly AsyncCachedLookup<string, CardTypeRecord> cardTypeRecordLookup;

        private readonly AsyncCachedLookup<string, SuperTypeRecord> superTypeRecordLookup;
        private readonly AsyncCachedLookup<string, MainTypeRecord> mainTypeRecordLookup;
        private readonly AsyncCachedLookup<string, SubTypeRecord> subTypeRecordLookup;
        private readonly AsyncCachedLookup<string, ManaSymbolRecord> manaSymbolRecordLookup;

        private readonly AsyncCachedLookup<string, RarityTypeRecord> rarityTypeRecordLookup;
        private readonly AsyncCachedLookup<string, SubsetTypeRecord> subsetTypeRecordLookup;
        private readonly AsyncCachedLookup<string, PrintingTypeRecord> printingTypeRecordLookup;

        private SetProcessor(AppDataContext dataContext, SetRecord setRecord, IProgress<Progress> progress)
        {
            this.dataContext = dataContext;
            this.setRecord = setRecord;
            this.progress = progress;

            cardTypeRecordLookup = new AsyncCachedLookup<string, CardTypeRecord>(
                key => dataContext.Cards.CardTypes
                    .QueryWhere(cardType => cardType.Value == key)
                    .SingleAsync());

            superTypeRecordLookup = new AsyncCachedLookup<string, SuperTypeRecord>(
                key => dataContext.Cards.SuperTypes
                    .QueryWhere(superType => superType.Value == key)
                    .TryGetSingleAsync());
            mainTypeRecordLookup = new AsyncCachedLookup<string, MainTypeRecord>(
                key => dataContext.Cards.MainTypes
                    .QueryWhere(mainType => mainType.Value == key)
                    .TryGetSingleAsync());
            subTypeRecordLookup = new AsyncCachedLookup<string, SubTypeRecord>(
                key => dataContext.Cards.SubTypes
                    .QueryWhere(sub => sub.Value == key)
                    .TryGetSingleAsync());
            manaSymbolRecordLookup = new AsyncCachedLookup<string, ManaSymbolRecord>(
                key => dataContext.Cards.ManaSymbols
                    .QueryWhere(manaSymbol => manaSymbol.Code == key)
                    .TryGetSingleAsync());

            rarityTypeRecordLookup = new AsyncCachedLookup<string, RarityTypeRecord>(
                key => dataContext.Cards.RarityTypes
                    .QueryWhere(rarityType => rarityType.Value == key)
                    .SingleAsync());
            subsetTypeRecordLookup = new AsyncCachedLookup<string, SubsetTypeRecord>(
                key => dataContext.Cards.SubsetTypes
                    .QueryWhere(subsetType => subsetType.Value == key)
                    .SingleAsync());
            printingTypeRecordLookup = new AsyncCachedLookup<string, PrintingTypeRecord>(
                key => dataContext.Cards.PrintingTypes
                    .QueryWhere(printingType => printingType.Value == key)
                    .SingleAsync());
        }

        public static SetProcessor Create(AppDataContext dataContext, SetRecord setRecord, IProgress<Progress> progress)
            => new SetProcessor(dataContext, setRecord, progress);

        public async Task ProcessAsync()
        {
            ReportProgress(0, 0, "Initializing...");

            var dataMapper = new GathererCardDataMapper(dataContext, progress, setRecord.Code, setRecord.CodeExtension, setRecord.Name);
            var cardData = await dataMapper.LoadAllCardData().ConfigureAwait(false);

            var processedCount = 0;
            var totalCount = cardData.Count();

            var cardDataProcessor = new CardDataProcessor(dataContext);
            foreach(var card in cardData)
            {
                var cardName = card.PrimaryCardFace.Name + (card.HasSecondaryCardFace ? $" // {card.SecondaryCardFace.Name}" : string.Empty);
                ReportProgress(++processedCount, totalCount, $"Updating records...");
                await cardDataProcessor.ProcessCard(card).ConfigureAwait(false);
            }

            ReportProgress(1, 1, "Completed!");
        }

        public async Task ProcessAsyncOld()
        {
            ReportProgress(0, 0, "Initializing...");
            var setAdapter = GathererSetAdapter.Create(setRecord.Name);
            var logicalOrdinal = 0;

            var imageDownloadTasks = new List<Task>();

            var current = 0;
            var total = await setAdapter.GetCardCountAsync()
                .ConfigureAwait(false);
            await foreach (var cardAdapter in setAdapter.Cards.ConfigureAwait(false))
            {
                ReportProgress(current++, total, $"Processing card ({current}/{total}): {cardAdapter.Name}");

                var linkedCardAdapter = cardAdapter is GathererMultiFaceCardAdapter multiFacedCardAdapter ?
                    await setAdapter.FindAdapterByNameWithClosestMultiverseId(multiFacedCardAdapter.LinkedFaceName, multiFacedCardAdapter.MultiverseId)
                        .ConfigureAwait(false) :
                    null;

                // Gatherer displays both meld cards as linked cards, but we link only those with a matching collector's number
                if (linkedCardAdapter != null && cardAdapter.CollectorsNumber != linkedCardAdapter.CollectorsNumber)
                    linkedCardAdapter = null;

                // Gatherer displays 'Battlebond' partner cards as a pair, but these should be treated differently
                if (setRecord.Name == "Battlebond")
                    linkedCardAdapter = null;

                var faceRecord = await FindOrCreateFaceRecord(cardAdapter, linkedCardAdapter)
                    .ConfigureAwait(false);

                if (cardAdapter.HasOracleText)
                {
                    var oracleTextResult = await dataContext.Cards.OracleText
                        .QueryWhere(oracleText => oracleText.FaceRecord == faceRecord)
                        .TryGetSingleAsync()
                        .ConfigureAwait(false);
                    if (!oracleTextResult.Success)
                        await dataContext.Cards.OracleText
                            .NewRecordAsync(faceRecord, cardAdapter.OracleText)
                            .ConfigureAwait(false);
                    else
                    {
                        var oracleTextRecord = oracleTextResult.Value;
                        oracleTextRecord.Value = cardAdapter.OracleText;
                        await oracleTextRecord.UpdateRecordAsync()
                            .ConfigureAwait(false);
                    }
                }

                if (cardAdapter.HasPowerAndToughness)
                {
                    var powerToughnessResult = await dataContext.Cards.PowerToughness
                        .QueryWhere(powerToughness => powerToughness.FaceRecord == faceRecord)
                        .TryGetSingleAsync()
                        .ConfigureAwait(false);
                    if (!powerToughnessResult.Success)
                        await dataContext.Cards.PowerToughness
                            .NewRecordAsync(faceRecord, cardAdapter.Power.Value, cardAdapter.Power.Format, cardAdapter.Toughness.Value, cardAdapter.Toughness.Format)
                            .ConfigureAwait(false);
                }

                if (cardAdapter.HasLoyalty)
                {
                    var loyaltyResult = await dataContext.Cards.Loyalty
                        .QueryWhere(loyalty => loyalty.FaceRecord == faceRecord)
                        .TryGetSingleAsync()
                        .ConfigureAwait(false);
                    if (!loyaltyResult.Success)
                        await dataContext.Cards.Loyalty
                            .NewRecordAsync(faceRecord, cardAdapter.Loyalty.Value, cardAdapter.Loyalty.Format)
                            .ConfigureAwait(false);
                }

                var identifiedTypes = new HashSet<string>();

                var requiredSuperTypeRecords = await cardAdapter.Types
                    .SelectAsync(async type => await superTypeRecordLookup.TryGetValueAsync(type).ConfigureAwait(false))
                    .WhereAsync(result => result.Success)
                    .SelectAsync(result => result.Value)
                    .ToListAsync()
                    .ConfigureAwait(false);
                identifiedTypes.UnionWith(requiredSuperTypeRecords.Select(record => record.Value));

                var requiredMainTypeRecords = await cardAdapter.Types
                    .SelectAsync(async type => await mainTypeRecordLookup.TryGetValueAsync(type).ConfigureAwait(false))
                    .WhereAsync(result => result.Success)
                    .SelectAsync(result => result.Value)
                    .ToListAsync()
                    .ConfigureAwait(false);
                identifiedTypes.UnionWith(requiredMainTypeRecords.Select(record => record.Value));

                var requiredSubTypeRecords = await cardAdapter.Types
                    .SelectAsync(async type => await subTypeRecordLookup.TryGetValueAsync(type).ConfigureAwait(false))
                    .WhereAsync(result => result.Success)
                    .SelectAsync(result => result.Value)
                    .ToListAsync()
                    .ConfigureAwait(false);
                identifiedTypes.UnionWith(requiredSubTypeRecords.Select(record => record.Value));

                var missingTypes = cardAdapter.Types
                    .Where(type => !identifiedTypes.Contains(type));
                if (missingTypes.Any())
                {
                    var formattedMissingTypes = string.Join(", ", missingTypes.Select(type => $"'{type}'"));
                    var s = missingTypes.Count() > 1 ? "s" : string.Empty;
                    throw new Exception($"The type{s} {formattedMissingTypes} could not be found in the database. You may need to add the record{s} manually.");
                }

                var faceHasSuperTypeRecords = await dataContext.Cards.FaceHasSuperTypes
                    .QueryWhere(record => record.FaceRecord == faceRecord)
                    .OrderBy(record => record.Ordinal, SortDirection.Ascending)
                    .ToListAsync()
                    .ConfigureAwait(false);
                var superTypeRecordsMatch = requiredSuperTypeRecords
                    .Select(record => record.Id)
                    .SequenceEqual(faceHasSuperTypeRecords.Select(record => record.SuperTypeId));
                if (!superTypeRecordsMatch)
                {
                    var ordinal = 1;
                    foreach (var record in faceHasSuperTypeRecords)
                        await record.DeleteRecordAsync()
                            .ConfigureAwait(false);
                    foreach (var superTypeRecord in requiredSuperTypeRecords)
                        await dataContext.Cards.FaceHasSuperTypes
                            .NewRecordAsync(faceRecord, superTypeRecord, ordinal++)
                            .ConfigureAwait(false);
                }

                var faceHasMainTypeRecords = await dataContext.Cards.FaceHasMainTypes
                    .QueryWhere(record => record.FaceRecord == faceRecord)
                    .OrderBy(record => record.Ordinal, SortDirection.Ascending)
                    .ToListAsync()
                    .ConfigureAwait(false);
                var mainTypeRecordsMatch = requiredMainTypeRecords
                    .Select(record => record.Id)
                    .SequenceEqual(faceHasMainTypeRecords.Select(record => record.MainTypeId));
                if (!mainTypeRecordsMatch)
                {
                    var ordinal = 1;
                    foreach (var record in faceHasMainTypeRecords)
                        await record.DeleteRecordAsync()
                            .ConfigureAwait(false);
                    foreach (var mainTypeRecord in requiredMainTypeRecords)
                        await dataContext.Cards.FaceHasMainTypes
                            .NewRecordAsync(faceRecord, mainTypeRecord, ordinal++)
                            .ConfigureAwait(false);
                }

                var faceHasSubTypeRecords = await dataContext.Cards.FaceHasSubTypes
                    .QueryWhere(record => record.FaceRecord == faceRecord)
                    .OrderBy(record => record.Ordinal, SortDirection.Ascending)
                    .ToListAsync()
                    .ConfigureAwait(false);
                var subTypeRecordsMatch = requiredSubTypeRecords
                    .Select(record => record.Id)
                    .SequenceEqual(faceHasSubTypeRecords.Select(record => record.SubTypeId));
                if (!subTypeRecordsMatch)
                {
                    var ordinal = 1;
                    foreach (var record in faceHasSubTypeRecords)
                        await record.DeleteRecordAsync()
                            .ConfigureAwait(false);
                    foreach (var subTypeRecord in requiredSubTypeRecords)
                        await dataContext.Cards.FaceHasSubTypes
                            .NewRecordAsync(faceRecord, subTypeRecord, ordinal++)
                            .ConfigureAwait(false);
                }

                var existingCastingCostRecords = await dataContext.Cards.FaceHasCastingCost
                    .QueryWhere(record => record.FaceRecord == faceRecord)
                    .OrderBy(record => record.Ordinal, SortDirection.Ascending)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var requiredManaSymbolRecords = await cardAdapter.CastingCost
                    .SelectAsync(async manaSymbol => await manaSymbolRecordLookup.TryGetValueAsync(manaSymbol.SymbolType).ConfigureAwait(false))
                    .WhereAsync(result => result.Success)
                    .SelectAsync(result => result.Value)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var missingManaSymbols = cardAdapter.CastingCost
                    .Where(castingCost => !requiredManaSymbolRecords.Select(record => record.Code).Contains(castingCost.SymbolType));
                if (missingManaSymbols.Any())
                {
                    var formattedManaSymbols = string.Join(", ", missingManaSymbols.Select(manaSymbol => $"'{manaSymbol.SymbolType}'"));
                    var s = missingManaSymbols.Count() > 1 ? "s" : string.Empty;
                    throw new Exception($"The mana symbol{s} {formattedManaSymbols} could not be found in the database. You may need to add the record{s} manually.");
                }

                var castingCostRecordsMatch = requiredManaSymbolRecords.Zip(cardAdapter.CastingCost, (record, castingCost) => (record.Id, castingCost.Count))
                    .SequenceEqual(existingCastingCostRecords.Select(record => (record.ManaSymbolId, record.Count)));
                if (!castingCostRecordsMatch)
                {
                    foreach (var existingRecord in existingCastingCostRecords)
                        await existingRecord.DeleteRecordAsync()
                            .ConfigureAwait(false);
                    var ordinal = 0;
                    foreach (var requiredCastingCost in cardAdapter.CastingCost)
                    {
                        var requiredManaSymbolRecord = requiredManaSymbolRecords
                            .Where(record => record.Code == requiredCastingCost.SymbolType)
                            .Single();
                        await dataContext.Cards.FaceHasCastingCost
                            .NewRecordAsync(faceRecord, requiredManaSymbolRecord, ordinal, requiredCastingCost.Count)
                            .ConfigureAwait(false);
                    }
                }

                var baseRecord = await dataContext.Cards.Bases
                    .QueryWhere(record => record.Id == faceRecord.BaseId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var subsetTypeRecord = await subsetTypeRecordLookup
                    .GetValueAsync("Main Set")
                    .ConfigureAwait(false);

                SetInclusionRecord setInclusionRecord;
                var setInclusionResult = cardAdapter.HasCollectorsNumber ?
                    
                    await dataContext.Cards.SetInclusions
                    .QueryWhere(record => 
                        record.SetRecord == setRecord &&
                        record.BaseRecord == baseRecord &&
                        record.SubsetTypeRecord == subsetTypeRecord &&
                        record.CollectorsNumber == cardAdapter.CollectorsNumber)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false) :
                    
                    await dataContext.Cards.SetInclusions
                        .QueryWhere(record =>
                            record.SetRecord == setRecord &&
                            record.BaseRecord == baseRecord &&
                            record.SubsetTypeRecord == subsetTypeRecord &&
                            record.LogicalOrdinal == logicalOrdinal)
                        .TryGetSingleAsync()
                        .ConfigureAwait(false);

                if (setInclusionResult.Success)
                    setInclusionRecord = setInclusionResult.Value;
                else
                {
                    var rarityTypeRecord = await rarityTypeRecordLookup
                        .GetValueAsync(cardAdapter.Rarity)
                        .ConfigureAwait(false);
                    var collectorsNumber = cardAdapter.HasCollectorsNumber ?
                        (short?)cardAdapter.CollectorsNumber :
                        null;
                    setInclusionRecord = await dataContext.Cards.SetInclusions
                        .NewRecordAsync(setRecord, subsetTypeRecord, baseRecord, rarityTypeRecord, logicalOrdinal, collectorsNumber);
                }

                var printingTypeRecord = await printingTypeRecordLookup
                    .GetValueAsync("Regular")
                    .ConfigureAwait(false);

                ArtistRecord artistRecord;
                var artistResult = await dataContext.Cards.Artists
                    .QueryWhere(record => record.Name == cardAdapter.Artist)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (artistResult.Success)
                    artistRecord = artistResult.Value;
                else
                    artistRecord = await dataContext.Cards.Artists
                        .NewRecordAsync(cardAdapter.Artist)
                        .ConfigureAwait(false);

                BasePrintingRecord basePrintingRecord;
                var basePrintingResult = await dataContext.Cards.BasePrintings
                    .QueryWhere(record => 
                        record.SetInclusionRecord == setInclusionRecord &&
                        record.PrintingTypeRecord == printingTypeRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (basePrintingResult.Success)
                    basePrintingRecord = basePrintingResult.Value;
                else
                    basePrintingRecord = await dataContext.Cards.BasePrintings
                        .NewRecordAsync(setInclusionRecord, printingTypeRecord, artistRecord)
                        .ConfigureAwait(false);

                FacePrintingRecord facePrintingRecord;
                var facePrintingResult = await dataContext.Cards.FacePrintings
                    .QueryWhere(record =>
                        record.BasePrintingRecord == basePrintingRecord &&
                        record.FaceRecord == faceRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (facePrintingResult.Success)
                    facePrintingRecord = facePrintingResult.Value;
                else
                    facePrintingRecord = await dataContext.Cards.FacePrintings
                        .NewRecordAsync(basePrintingRecord, faceRecord)
                        .ConfigureAwait(false);

                if (cardAdapter.HasFlavorText)
                {
                    var flavorTextResult = await dataContext.Cards.FlavorText
                        .QueryWhere(record => record.FacePrintingRecord == facePrintingRecord)
                        .TryGetSingleAsync()
                        .ConfigureAwait(false);
                    if (flavorTextResult.Success)
                    {
                        var flavorTextRecord = flavorTextResult.Value;
                        flavorTextRecord.Value = cardAdapter.FlavorText;
                        await flavorTextRecord.UpdateRecordAsync()
                            .ConfigureAwait(false);
                    }
                    else
                        await dataContext.Cards.FlavorText
                            .NewRecordAsync(facePrintingRecord, cardAdapter.FlavorText)
                            .ConfigureAwait(false);
                }

                var imageUrl = $"http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid={cardAdapter.MultiverseId}&type=card";
                var imageDirectory = Path.Combine(
                    @"C:\Users\mmhar\Source\Repos\supreme-dollop\Web\App\Content\Images\CardImages",
                    setRecord.Id.ToString(),
                    printingTypeRecord.Id.ToString());
                var saveLocation = Path.Combine(imageDirectory, $"{facePrintingRecord.ImageId}.png");
                if (!Directory.Exists(imageDirectory))
                    Directory.CreateDirectory(imageDirectory);
                if (!File.Exists(saveLocation))
                    imageDownloadTasks.Add(DownloadImageAsync(imageUrl, saveLocation));

                logicalOrdinal++;
            }
            
            await Task.WhenAll(imageDownloadTasks).ConfigureAwait(false);

            ReportProgress(1, 1, "Complete!");

            async Task<FaceRecord> FindOrCreateFaceRecord(GathererCardAdapter cardAdapter, GathererCardAdapter? linkedCardAdapter)
            {
                var faceResult = await dataContext.Cards.Faces
                    .QueryWhere(face => face.Name == cardAdapter.Name)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (faceResult.Success)
                    return faceResult.Value;

                if (linkedCardAdapter != null)
                {
                    var linkedFaceResult = await dataContext.Cards.Faces
                        .QueryWhere(face => face.Name == linkedCardAdapter.Name)
                        .TryGetSingleAsync()
                        .ConfigureAwait(false);
                    if (linkedFaceResult.Success)
                    {
                        var existingBaseRecord = await dataContext.Cards.Bases
                            .QueryWhere(baseRecord => baseRecord.Id == linkedFaceResult.Value.BaseId)
                            .SingleAsync()
                            .ConfigureAwait(false);
                        return await dataContext.Cards.Faces
                            .NewRecordAsync(existingBaseRecord, cardAdapter.Name, cardAdapter.IsPrimaryFace)
                            .ConfigureAwait(false);
                    }
                }

                var cardType = DetermineCardType(false, cardAdapter);
                if (linkedCardAdapter != null && linkedCardAdapter.IsPrimaryFace)
                    cardType = DetermineCardType(true, linkedCardAdapter);
                var cardTypeResult = await cardTypeRecordLookup
                    .TryGetValueAsync(cardType)
                    .ConfigureAwait(false);
                if (!cardTypeResult.Success)
                    throw new Exception($"Failed to find card type '{cardType}' in the database.");
                var newBaseRecord = await dataContext.Cards.Bases
                    .NewRecordAsync(cardTypeResult.Value)
                    .ConfigureAwait(false);
                return await dataContext.Cards.Faces
                    .NewRecordAsync(newBaseRecord, cardAdapter.Name, cardAdapter.IsPrimaryFace)
                    .ConfigureAwait(false);

                string DetermineCardType(bool isLinkedCard, GathererCardAdapter primaryFaceAdapter)
                {
                    if (!isLinkedCard)
                        return "SingleFaced";
                    if (primaryFaceAdapter.HasOracleText)
                    {
                        if (primaryFaceAdapter.OracleText.ToLowerInvariant().Contains("transform"))
                            return "Transform";
                        if (primaryFaceAdapter.OracleText.ToLowerInvariant().Contains("meld"))
                            return "Meld";
                        if (primaryFaceAdapter.OracleText.ToLowerInvariant().Contains("flip"))
                            return "Flip";
                        if (primaryFaceAdapter.OracleText.ToLowerInvariant().Contains("fuse"))
                            return "Split";
                    }
                    if (setRecord.Name == "Amonkhet" || setRecord.Name == "Hour of Devastation")
                        return "Aftermath";
                    if (setRecord.Name == "Throne of Eldraine")
                        return "Adventure";
                    return "Uncategorized";
                }
            }

            static async Task DownloadImageAsync(string imageUrl, string saveLocation)
            {
                var exceptions = new List<Exception>();
                for (int i = 0; i < 5; i++)
                    try
                    {
                        using (var client = new WebClient())
                            await client.DownloadFileTaskAsync(new Uri(imageUrl), saveLocation)
                                .ConfigureAwait(false);
                        return;
                    }
                    catch (WebException ex)
                    {
                        exceptions.Add(ex);
                        var response = (HttpWebResponse)ex.Response;
                        if (response != null && response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            if (File.Exists(imageUrl))
                                File.Delete(imageUrl);
                            await Task.Delay(5000).ConfigureAwait(false);
                            continue;
                        }
                        break;
                    }
                throw new AggregateException(exceptions);
            }
        }

        private void ReportProgress(int current, int total, string? detail = null)
            => progress.Report(Progress.Create($"Processing set: {setRecord.Name}", current, total, detail));
    }
}
