using MindSculptor.App.AppDataContext;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mindsculptor.Tools.Applications.MtgaLinker
{
    class Program
    {
        private Program()
        {

        }

        static void Main(string[] args)
        {
            var program = new Program();
            program.Run().Wait();
        }

        private async Task Run()
        {
            const string mtgaDataDirectoryPath = @"C:\Program Files (x86)\Wizards of the Coast\MTGA\MTGA_Data\Downloads\Data\";
            const string cardsDataFileName = @"data_cards_4dcb7721e148573e66a4dcb5ec4e871e.mtga";
            const string locDataFileName = @"data_loc_3b2cf3c5472bf8418661b4a12155d3e8.mtga";

            const string dbConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MindSculptorApp002;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            var cardsDataFilePath = Path.Combine(mtgaDataDirectoryPath, cardsDataFileName);
            var locDataFilePath = Path.Combine(mtgaDataDirectoryPath, locDataFileName);

            if (File.Exists(cardsDataFilePath) && File.Exists(locDataFilePath))
            {
                List<CardsData> cardsData;
                List<TextLanguage> locData;
                
                using (var fileStream = File.OpenRead(cardsDataFilePath))
                    cardsData = await JsonSerializer.DeserializeAsync<List<CardsData>>(fileStream);
                using (var fileStream = File.OpenRead(locDataFilePath))
                    locData = await JsonSerializer.DeserializeAsync<List<TextLanguage>>(fileStream);

                var enUSLocData = locData
                    .Where(loc => loc.isoCode == "en-US")
                    .Single();

                var titledCardData = cardsData
                    .Where(data => 
                    {
                        if (data.isCollectible && data.isCraftable && !data.isToken)
                            return true;
                        if (new[] { "Plains", "Island", "Swamp", "Mountain", "Forest" }.Contains(enUSLocData.keys.Where(key => key.id == data.titleId).Single().text))
                            return true;
                        return false;
                    })
                    .Select(
                    data => new
                    {
                        Id = data.grpid,
                        Name = enUSLocData.keys.Where(key => key.id == data.titleId)
                            .Single()
                            .text,
                        Set = data.set,
                        CollectorsNumber = Int32.TryParse(data.CollectorNumber, out var parsedValue) ? (int?)parsedValue : null,
                        LinkedFaceNames = data.linkedFaces.Select(faceId => 
                        {
                            var linkedtitleId = cardsData.Where(x => x.grpid == faceId).Single().titleId;
                            return enUSLocData.keys.Where(key => key.id == linkedtitleId).Single().text;
                        })
                    })
                    .Where( data => data.CollectorsNumber != null);

                using (var dbConnection = new SqlConnection(dbConnectionString))
                    try
                    {
                        await dbConnection.OpenAsync();
                        using (var dbTransaction = await dbConnection.BeginTransactionAsync(IsolationLevel.Serializable))
                            try
                            {
                                var dataContext = AppDataContext.Create(dbConnection, dbTransaction);

                                var subsetTypeRecord = await dataContext.Cards.SubsetTypes
                                    .QueryWhere(record => record.Value == "Main Set")
                                    .SingleAsync();
                                var digitalPrintingTypeRecord = await dataContext.Cards.PrintingTypes
                                    .QueryWhere(record => record.Value == "Digital")
                                    .SingleAsync();
                                var regularPrintingTypeRecord = await dataContext.Cards.PrintingTypes
                                    .QueryWhere(record => record.Value == "Regular")
                                    .SingleAsync();

                                foreach (var cardDatum in titledCardData)
                                {
                                    var lookupName = cardDatum.LinkedFaceNames.Any() ?
                                        cardDatum.LinkedFaceNames.First() :
                                        cardDatum.Name;

                                    var faceRecordResult = await dataContext.Cards.Faces
                                        .QueryWhere(faceRecord => faceRecord.Name == lookupName)
                                        .TryGetSingleAsync();
                                    if (!faceRecordResult.Success)
                                        continue;

                                    var faceRecord = faceRecordResult.Value;
                                    var baseRecord = await dataContext.Cards.Bases
                                        .QueryWhere(baseRecord => baseRecord.Id == faceRecord.BaseId)
                                        .SingleAsync();

                                    var setResult = await dataContext.Cards.Sets
                                        .QueryWhere(setDetialRecord => setDetialRecord.Code == cardDatum.Set)
                                        .TryGetSingleAsync();
                                    if (!setResult.Success)
                                        continue;

                                    var setRecord = setResult.Value;
                                    var setInclusionResult = await dataContext.Cards.SetInclusions
                                        .QueryWhere(record =>
                                            record.SubsetTypeRecord == subsetTypeRecord &&
                                            record.SetRecord == setRecord &&
                                            record.BaseRecord == baseRecord &&
                                            record.CollectorsNumber == cardDatum.CollectorsNumber)
                                        .TryGetSingleAsync();
                                    if (!setInclusionResult.Success)
                                        continue;

                                    BasePrintingRecord digitalBasePrintingRecord;
                                    var setInclusionRecord = setInclusionResult.Value;
                                    var digitalBasePrintingResult = await dataContext.Cards.BasePrintings
                                        .QueryWhere(record =>
                                            record.SetInclusionRecord == setInclusionRecord &&
                                            record.PrintingTypeRecord == digitalPrintingTypeRecord)
                                        .TryGetSingleAsync();
                                    if (!digitalBasePrintingResult.Success) 
                                    {
                                        var regularBasePrintingResult = await dataContext.Cards.BasePrintings
                                            .QueryWhere(record =>
                                                record.SetInclusionRecord == setInclusionRecord &&
                                                record.PrintingTypeRecord == regularPrintingTypeRecord)
                                            .TryGetSingleAsync();
                                        if (!regularBasePrintingResult.Success)
                                            continue;

                                        digitalBasePrintingRecord = await dataContext.Cards
                                            .BasePrintings.NewRecordAsync(setInclusionRecord, digitalPrintingTypeRecord);
                                    }
                                    else
                                        digitalBasePrintingRecord = digitalBasePrintingResult.Value;

                                    var cardResult = await dataContext.Mtga.Cards
                                        .QueryWhere(record => record.MtgaCardId == cardDatum.Id)
                                        .TryGetSingleAsync();
                                    if (cardResult.Success)
                                        continue;

                                    await dataContext.Mtga.Cards
                                        .NewRecordAsync(digitalBasePrintingRecord, cardDatum.Id);
                                }

                                await dbTransaction.CommitAsync();
                            }
                            catch
                            {
                                await dbTransaction.RollbackAsync();
                            }
                    }
                    finally
                    {
                        await dbConnection.CloseAsync();
                    }
            }
            else
                throw new Exception("Data file does not exit.");
        }
    }

    class CardsData
    {
        public int grpid { get; set; }
        public int titleId { get; set; }
        public string set { get; set; }
        public string CollectorNumber { get; set; }
        public bool isToken { get; set; }
        public bool isCollectible { get; set; }
        public bool isCraftable { get; set; }
        public List<int> linkedFaces { get; set; }
    }

    class TextLanguage
    {
        public string isoCode { get; set; }
        public List<TextValue> keys { get; set; }
    }

    class TextValue
    {
        public int id { get; set; }
        public string text { get; set; }
    }
}
