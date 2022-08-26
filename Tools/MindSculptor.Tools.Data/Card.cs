using MindSculptor.Tools.Exceptions;

namespace MindSculptor.Tools.Data
{
    public class Card
    {
        private readonly string? setCodeExtension;
        private readonly int? collectorsNumber;
        private readonly CardFace? secondaryCardFace;
        private readonly int? mtgaCardId;

        public bool HasSetCodeExtension => setCodeExtension != null;
        public bool HasCollectorsNumber => collectorsNumber != null;
        public bool HasSecondaryCardFace => secondaryCardFace != null;
        public bool HasMtgaCardId => mtgaCardId != null;

        public string CardType { get; }
        public string SubsetType { get; }
        public string PrintingType { get; }

        public string SetCode { get; }
        public string SetCodeExtension => setCodeExtension ?? throw new PropertyUndefinedException(nameof(SetCodeExtension), nameof(HasSetCodeExtension));
        public int CollectorsNumber => collectorsNumber ?? throw new PropertyUndefinedException(nameof(CollectorsNumber), nameof(HasCollectorsNumber));
        public string Rarity { get; }
        public string Artist { get; }

        public CardFace PrimaryCardFace { get; }
        public CardFace SecondaryCardFace => secondaryCardFace ?? throw new PropertyUndefinedException(nameof(SecondaryCardFace), nameof(HasSecondaryCardFace));

        public int MtgaCardId => mtgaCardId ?? throw new PropertyUndefinedException(nameof(MtgaCardId), nameof(HasMtgaCardId));

        public Card(string cardType, string subsetType, string printingType, string setCode, string? setCodeExtension, int? collectorsNumber, string rarity, string artist, 
            CardFace primaryCardFace, CardFace? secondaryCardFace, int? mtgaCardId)
        {
            CardType                = cardType;
            SubsetType              = subsetType;
            PrintingType            = printingType;
            SetCode                 = setCode;
            this.setCodeExtension   = setCodeExtension;
            this.collectorsNumber   = collectorsNumber;
            Rarity                  = rarity;
            Artist                  = artist;
            PrimaryCardFace         = primaryCardFace;
            this.secondaryCardFace  = secondaryCardFace;
            this.mtgaCardId         = mtgaCardId;
        }

        public static string DetermineCardType(bool isLinkedCard, CardFace primaryCardFace, string setCode)
        {
            if (!isLinkedCard)
                return "SingleFaced";
            if (primaryCardFace.HasOracleText)
            {
                if (primaryCardFace.OracleText.ToLowerInvariant().Contains("transform"))
                    return "Transform";
                if (primaryCardFace.OracleText.ToLowerInvariant().Contains("meld"))
                    return "Meld";
                if (primaryCardFace.OracleText.ToLowerInvariant().Contains("flip"))
                    return "Flip";
                if (primaryCardFace.OracleText.ToLowerInvariant().Contains("fuse"))
                    return "Split";
            }
            if (setCode == "AKH" || setCode == "HOU")
                return "Aftermath";
            if (setCode == "GRN" || setCode == "RNA")
                return "Split";
            if (setCode == "ELD")
                return "Adventure";
            return "Uncategorized";
        }
    }
}
