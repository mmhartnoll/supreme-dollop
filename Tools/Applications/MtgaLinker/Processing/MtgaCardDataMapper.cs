using MindSculptor.Tools.Data;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mindsculptor.Tools.Applications.MtgaLinker.Processing
{
    internal class MtgaCardDataMapper
    {
        public IEnumerable<Card> LoadAllCardData()
        {
            var orderedCardAdapters = LoadCardAdapters().OrderBy(adapter => adapter.MtgaCardId);
            foreach (var setGroup in orderedCardAdapters.GroupBy(adapter => adapter.SetCode))
                foreach (var collectorsNumberGroup in setGroup.AsEnumerable().GroupBy(adapter => adapter.CollectorsNumber))
                {
                    var cardAdapters = collectorsNumberGroup.OrderBy(adapter => adapter.MtgaCardId).ToArray();
                    if (cardAdapters.Count() == 2 && cardAdapters.Select(adapter => adapter.CollectorsNumber).Distinct().Count() == 1)
                    {
                        yield return new Card("SingleFaced", "Main Set", "Digital", cardAdapters[0].SetCode, null, cardAdapters[0].CollectorsNumber, cardAdapters[0].Rarity, cardAdapters[0].Artist,
                            MapCardFace(cardAdapters[0]), null, cardAdapters[0].MtgaCardId);

                        yield return new Card("SingleFaced", "Uncategorized", "Digital", cardAdapters[1].SetCode, null, cardAdapters[1].CollectorsNumber, cardAdapters[1].Rarity, cardAdapters[1].Artist,
                            MapCardFace(cardAdapters[1]), null, cardAdapters[1].MtgaCardId);
                    }
                    else
                    {
                        (int, int?) cardAdapterIndices = cardAdapters.Count() switch
                        {
                            1 => (0, null),
                            2 => (0, 1),
                            3 => (1, 2),
                            _ => throw new Exception(),
                        };

                        var primaryCardFace = MapCardFace(cardAdapters[cardAdapterIndices.Item1]);
                        var secondaryCardFace = cardAdapterIndices.Item2.HasValue ? MapCardFace(cardAdapters[cardAdapterIndices.Item2.Value]) : null;
                        var cardType = Card.DetermineCardType(secondaryCardFace != null, primaryCardFace, cardAdapters[0].SetCode);

                        yield return new Card(cardType, "Main Set", "Digital", cardAdapters[0].SetCode, null, cardAdapters[0].CollectorsNumber, cardAdapters[0].Rarity, cardAdapters[0].Artist,
                            primaryCardFace, secondaryCardFace, cardAdapters[0].MtgaCardId);
                    }
                }
        }

        private CardFace MapCardFace(MtgaCardAdapter cardAdapter)
        {
            var oracleText  = cardAdapter.HasOracleText         ? cardAdapter.OracleText : null;
            var power       = cardAdapter.HasPowerAndToughness  ? cardAdapter.Power : null;
            var toughness   = cardAdapter.HasPowerAndToughness  ? cardAdapter.Toughness : null;
            var loyalty     = cardAdapter.HasLoyalty            ? cardAdapter.Loyalty : null;
            var flavorText  = cardAdapter.HasFlavorText         ? cardAdapter.FlavorText : null;

            return new CardFace(cardAdapter.Name, cardAdapter.CastingCost, cardAdapter.SuperTypes, cardAdapter.MainTypes, cardAdapter.SubTypes, oracleText, power, toughness, loyalty, flavorText);
        }

        private IEnumerable<MtgaCardAdapter> LoadCardAdapters()
        {
            var cardAdapters = new List<MtgaCardAdapter>();
            var cardsDocument = JsonDocumentLoader.LoadJsonDocument(CardsFileRegexPattern);
            foreach (var cardElement in cardsDocument.RootElement.EnumerateArray())
            {
                if (MtgaCardAdapter.TryGetAdapter(cardElement, out var cardAdapter))
                    cardAdapters.Add(cardAdapter);
            }
            return cardAdapters.Enumerate();
        }

        private const string CardsFileRegexPattern = @"^data_cards_[\da-f]{32}.mtga$";
    }
}
