using Mindsculptor.Tools.Applications.MtgaLinker.Processing;
using MindSculptor.Tools.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace Mindsculptor.Tools.Applications.MtgaLinker
{
    class Program
    {
        static async Task Main(string[] _)
        {
            var cards = new MtgaCardDataMapper().LoadAllCardData();

            using var dataContext = DataContext.Create(DBConnectionString);
            var processor = new CardDataProcessor(dataContext);

            var failedCards = new List<Card>();
            foreach (var card in cards)
            {
                var transactionScope = await dataContext.BeginTransactionAsync().ConfigureAwait(false);
                await using (transactionScope.ConfigureAwait(false))
                    try
                    {
                        await transactionScope.ExecuteAsync(TransactionScope).ConfigureAwait(false);
                        await transactionScope.CommitAsync().ConfigureAwait(false);
                    }
                    catch
                    {
                        await transactionScope.RollbackAsync().ConfigureAwait(false);
                        failedCards.Add(card);
                    }

                Task TransactionScope()
                    => processor.ProcessCard(card);
            }

            using var streamWriter = File.CreateText(@"C:\Users\mmhar\OneDrive\Desktop\linker_failed_cards.txt");
            foreach (var failedCard in failedCards)
            {
                var name = failedCard.HasSecondaryCardFace ? $"{failedCard.PrimaryCardFace.Name} // {failedCard.SecondaryCardFace.Name}" : failedCard.PrimaryCardFace.Name;

                streamWriter.WriteLine(failedCard.MtgaCardId);
                streamWriter.WriteLine(name);
                streamWriter.WriteLine(failedCard.SetCode);
                streamWriter.WriteLine(failedCard.CollectorsNumber);
                streamWriter.WriteLine(failedCard.Artist);
                streamWriter.WriteLine();
            }
        }

        private const string DBConnectionString = @"Server=localhost\SQLEXPRESS;Database=MindSculptorApp;Trusted_Connection=True;";
    }
}
