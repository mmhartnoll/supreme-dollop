using MindSculptor.App.AppDataContext.Schemas.Cards.Tables;
using MindSculptor.DataAccess.DataContext;
using System;

namespace MindSculptor.App.AppDataContext.Schemas
{
    public class CardsSchema : DataContextSchema
    {
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

        private CardsSchema(DataContext dataContext) : base(dataContext)
        {
            basePrintingsTableLoader = new Lazy<BasePrintingsTable>(() => BasePrintingsTable.Create(DataContext));
            basesTableLoader = new Lazy<BasesTable>(() => BasesTable.Create(DataContext));
            cardTypesTableLoader = new Lazy<CardTypesTable>(() => CardTypesTable.Create(DataContext));
            faceHasCastingCostTableLoader = new Lazy<FaceHasCastingCostTable>(() => FaceHasCastingCostTable.Create(DataContext));
            faceHasMainTypesTableLoader = new Lazy<FaceHasMainTypesTable>(() => FaceHasMainTypesTable.Create(DataContext));
            faceHasSubTypesTableLoader = new Lazy<FaceHasSubTypesTable>(() => FaceHasSubTypesTable.Create(DataContext));
            faceHasSuperTypesTableLoader = new Lazy<FaceHasSuperTypesTable>(() => FaceHasSuperTypesTable.Create(DataContext));
            facePrintingsTableLoader = new Lazy<FacePrintingsTable>(() => FacePrintingsTable.Create(DataContext));
            facesTableLoader = new Lazy<FacesTable>(() => FacesTable.Create(DataContext));
            flavorTextTableLoader = new Lazy<FlavorTextTable>(() => FlavorTextTable.Create(DataContext));
            loyaltyTableLoader = new Lazy<LoyaltyTable>(() => LoyaltyTable.Create(DataContext));
            mainTypesTableLoader = new Lazy<MainTypesTable>(() => MainTypesTable.Create(DataContext));
            manaSymbolsTableLoader = new Lazy<ManaSymbolsTable>(() => ManaSymbolsTable.Create(DataContext));
            oracleTextTableLoader = new Lazy<OracleTextTable>(() => OracleTextTable.Create(DataContext));
            powerToughnessTableLoader = new Lazy<PowerToughnessTable>(() => PowerToughnessTable.Create(DataContext));
            printingTypesTableLoader = new Lazy<PrintingTypesTable>(() => PrintingTypesTable.Create(DataContext));
            rarityTypesTableLoader = new Lazy<RarityTypesTable>(() => RarityTypesTable.Create(DataContext));
            setInclusionsTableLoader = new Lazy<SetInclusionsTable>(() => SetInclusionsTable.Create(DataContext));
            setsTableLoader = new Lazy<SetsTable>(() => SetsTable.Create(DataContext));
            subsetTypesTableLoader = new Lazy<SubsetTypesTable>(() => SubsetTypesTable.Create(DataContext));
            subTypesTableLoader = new Lazy<SubTypesTable>(() => SubTypesTable.Create(DataContext));
            superTypesTableLoader = new Lazy<SuperTypesTable>(() => SuperTypesTable.Create(DataContext));
        }

        internal static CardsSchema Create(DataContext dataContext)
        {
            return new CardsSchema(dataContext);
        }
    }
}