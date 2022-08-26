using MindSculptor.App.AppDataContext.Schemas.Cards.Tables;
using MindSculptor.DataAccess.Context;
using System;

namespace MindSculptor.App.AppDataContext.Schemas
{
    public class CardsSchema : DatabaseSchema
    {
        private Lazy<ArtistsTable> artistsTableLoader;
        private Lazy<BasePrintingsTable> basePrintingsTableLoader;
        private Lazy<BasesTable> basesTableLoader;
        private Lazy<CardTypesTable> cardTypesTableLoader;
        private Lazy<FaceHasCastingCostTable> faceHasCastingCostTableLoader;
        private Lazy<FaceHasMainTypesTable> faceHasMainTypesTableLoader;
        private Lazy<FaceHasSubTypesTable> faceHasSubTypesTableLoader;
        private Lazy<FaceHasSuperTypesTable> faceHasSuperTypesTableLoader;
        private Lazy<FacePrintingsTable> facePrintingsTableLoader;
        private Lazy<FacesTable> facesTableLoader;
        private Lazy<FlavorTextTable> flavorTextTableLoader;
        private Lazy<LoyaltyTable> loyaltyTableLoader;
        private Lazy<MainTypesTable> mainTypesTableLoader;
        private Lazy<ManaSymbolsTable> manaSymbolsTableLoader;
        private Lazy<OracleTextTable> oracleTextTableLoader;
        private Lazy<PowerToughnessTable> powerToughnessTableLoader;
        private Lazy<PrintingTypesTable> printingTypesTableLoader;
        private Lazy<RarityTypesTable> rarityTypesTableLoader;
        private Lazy<SetInclusionsTable> setInclusionsTableLoader;
        private Lazy<SetsTable> setsTableLoader;
        private Lazy<SubsetTypesTable> subsetTypesTableLoader;
        private Lazy<SubTypesTable> subTypesTableLoader;
        private Lazy<SuperTypesTable> superTypesTableLoader;

        public ArtistsTable Artists => artistsTableLoader.Value;

        public BasePrintingsTable BasePrintings => basePrintingsTableLoader.Value;

        public BasesTable Bases => basesTableLoader.Value;

        public CardTypesTable CardTypes => cardTypesTableLoader.Value;

        public FaceHasCastingCostTable FaceHasCastingCost => faceHasCastingCostTableLoader.Value;

        public FaceHasMainTypesTable FaceHasMainTypes => faceHasMainTypesTableLoader.Value;

        public FaceHasSubTypesTable FaceHasSubTypes => faceHasSubTypesTableLoader.Value;

        public FaceHasSuperTypesTable FaceHasSuperTypes => faceHasSuperTypesTableLoader.Value;

        public FacePrintingsTable FacePrintings => facePrintingsTableLoader.Value;

        public FacesTable Faces => facesTableLoader.Value;

        public FlavorTextTable FlavorText => flavorTextTableLoader.Value;

        public LoyaltyTable Loyalty => loyaltyTableLoader.Value;

        public MainTypesTable MainTypes => mainTypesTableLoader.Value;

        public ManaSymbolsTable ManaSymbols => manaSymbolsTableLoader.Value;

        public OracleTextTable OracleText => oracleTextTableLoader.Value;

        public PowerToughnessTable PowerToughness => powerToughnessTableLoader.Value;

        public PrintingTypesTable PrintingTypes => printingTypesTableLoader.Value;

        public RarityTypesTable RarityTypes => rarityTypesTableLoader.Value;

        public SetInclusionsTable SetInclusions => setInclusionsTableLoader.Value;

        public SetsTable Sets => setsTableLoader.Value;

        public SubsetTypesTable SubsetTypes => subsetTypesTableLoader.Value;

        public SubTypesTable SubTypes => subTypesTableLoader.Value;

        public SuperTypesTable SuperTypes => superTypesTableLoader.Value;

        private CardsSchema(DatabaseContext dataContext) : base(dataContext)
        {
            artistsTableLoader = new Lazy<ArtistsTable>(() => ArtistsTable.Create(Context));
            basePrintingsTableLoader = new Lazy<BasePrintingsTable>(() => BasePrintingsTable.Create(Context));
            basesTableLoader = new Lazy<BasesTable>(() => BasesTable.Create(Context));
            cardTypesTableLoader = new Lazy<CardTypesTable>(() => CardTypesTable.Create(Context));
            faceHasCastingCostTableLoader = new Lazy<FaceHasCastingCostTable>(() => FaceHasCastingCostTable.Create(Context));
            faceHasMainTypesTableLoader = new Lazy<FaceHasMainTypesTable>(() => FaceHasMainTypesTable.Create(Context));
            faceHasSubTypesTableLoader = new Lazy<FaceHasSubTypesTable>(() => FaceHasSubTypesTable.Create(Context));
            faceHasSuperTypesTableLoader = new Lazy<FaceHasSuperTypesTable>(() => FaceHasSuperTypesTable.Create(Context));
            facePrintingsTableLoader = new Lazy<FacePrintingsTable>(() => FacePrintingsTable.Create(Context));
            facesTableLoader = new Lazy<FacesTable>(() => FacesTable.Create(Context));
            flavorTextTableLoader = new Lazy<FlavorTextTable>(() => FlavorTextTable.Create(Context));
            loyaltyTableLoader = new Lazy<LoyaltyTable>(() => LoyaltyTable.Create(Context));
            mainTypesTableLoader = new Lazy<MainTypesTable>(() => MainTypesTable.Create(Context));
            manaSymbolsTableLoader = new Lazy<ManaSymbolsTable>(() => ManaSymbolsTable.Create(Context));
            oracleTextTableLoader = new Lazy<OracleTextTable>(() => OracleTextTable.Create(Context));
            powerToughnessTableLoader = new Lazy<PowerToughnessTable>(() => PowerToughnessTable.Create(Context));
            printingTypesTableLoader = new Lazy<PrintingTypesTable>(() => PrintingTypesTable.Create(Context));
            rarityTypesTableLoader = new Lazy<RarityTypesTable>(() => RarityTypesTable.Create(Context));
            setInclusionsTableLoader = new Lazy<SetInclusionsTable>(() => SetInclusionsTable.Create(Context));
            setsTableLoader = new Lazy<SetsTable>(() => SetsTable.Create(Context));
            subsetTypesTableLoader = new Lazy<SubsetTypesTable>(() => SubsetTypesTable.Create(Context));
            subTypesTableLoader = new Lazy<SubTypesTable>(() => SubTypesTable.Create(Context));
            superTypesTableLoader = new Lazy<SuperTypesTable>(() => SuperTypesTable.Create(Context));
        }

        internal static CardsSchema Create(DatabaseContext dataContext)
        {
            return new CardsSchema(dataContext);
        }
    }
}