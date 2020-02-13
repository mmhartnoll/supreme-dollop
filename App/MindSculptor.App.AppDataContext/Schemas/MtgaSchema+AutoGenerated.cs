using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables;
using MindSculptor.DataAccess.DataContext;
using System;

namespace MindSculptor.App.AppDataContext.Schemas
{
    public class MtgaSchema : DataContextSchema
    {
        private Lazy<ActiveDraftEventEntriesTable> activeDraftEventEntriesTableLoader;
        private Lazy<BoostersTable> boostersTableLoader;
        private Lazy<CardsTable> cardsTableLoader;
        private Lazy<DraftEventEntriesTable> draftEventEntriesTableLoader;
        private Lazy<DraftEventGamesTable> draftEventGamesTableLoader;
        private Lazy<DraftEventMatchesTable> draftEventMatchesTableLoader;
        private Lazy<DraftEventMatchResultsTable> draftEventMatchResultsTableLoader;
        private Lazy<DraftEventsTable> draftEventsTableLoader;
        private Lazy<DraftPicksTable> draftPicksTableLoader;
        private Lazy<OpponentsTable> opponentsTableLoader;
        private Lazy<ProfileHasBoostersTable> profileHasBoostersTableLoader;
        private Lazy<ProfileHasCardsTable> profileHasCardsTableLoader;
        private Lazy<ProfileInventoriesTable> profileInventoriesTableLoader;
        private Lazy<ProfilesTable> profilesTableLoader;

        public ActiveDraftEventEntriesTable ActiveDraftEventEntries => activeDraftEventEntriesTableLoader.Value;

        public BoostersTable Boosters => boostersTableLoader.Value;

        public CardsTable Cards => cardsTableLoader.Value;

        public DraftEventEntriesTable DraftEventEntries => draftEventEntriesTableLoader.Value;

        public DraftEventGamesTable DraftEventGames => draftEventGamesTableLoader.Value;

        public DraftEventMatchesTable DraftEventMatches => draftEventMatchesTableLoader.Value;

        public DraftEventMatchResultsTable DraftEventMatchResults => draftEventMatchResultsTableLoader.Value;

        public DraftEventsTable DraftEvents => draftEventsTableLoader.Value;

        public DraftPicksTable DraftPicks => draftPicksTableLoader.Value;

        public OpponentsTable Opponents => opponentsTableLoader.Value;

        public ProfileHasBoostersTable ProfileHasBoosters => profileHasBoostersTableLoader.Value;

        public ProfileHasCardsTable ProfileHasCards => profileHasCardsTableLoader.Value;

        public ProfileInventoriesTable ProfileInventories => profileInventoriesTableLoader.Value;

        public ProfilesTable Profiles => profilesTableLoader.Value;

        private MtgaSchema(DataContext dataContext) : base(dataContext)
        {
            activeDraftEventEntriesTableLoader = new Lazy<ActiveDraftEventEntriesTable>(() => ActiveDraftEventEntriesTable.Create(DataContext));
            boostersTableLoader = new Lazy<BoostersTable>(() => BoostersTable.Create(DataContext));
            cardsTableLoader = new Lazy<CardsTable>(() => CardsTable.Create(DataContext));
            draftEventEntriesTableLoader = new Lazy<DraftEventEntriesTable>(() => DraftEventEntriesTable.Create(DataContext));
            draftEventGamesTableLoader = new Lazy<DraftEventGamesTable>(() => DraftEventGamesTable.Create(DataContext));
            draftEventMatchesTableLoader = new Lazy<DraftEventMatchesTable>(() => DraftEventMatchesTable.Create(DataContext));
            draftEventMatchResultsTableLoader = new Lazy<DraftEventMatchResultsTable>(() => DraftEventMatchResultsTable.Create(DataContext));
            draftEventsTableLoader = new Lazy<DraftEventsTable>(() => DraftEventsTable.Create(DataContext));
            draftPicksTableLoader = new Lazy<DraftPicksTable>(() => DraftPicksTable.Create(DataContext));
            opponentsTableLoader = new Lazy<OpponentsTable>(() => OpponentsTable.Create(DataContext));
            profileHasBoostersTableLoader = new Lazy<ProfileHasBoostersTable>(() => ProfileHasBoostersTable.Create(DataContext));
            profileHasCardsTableLoader = new Lazy<ProfileHasCardsTable>(() => ProfileHasCardsTable.Create(DataContext));
            profileInventoriesTableLoader = new Lazy<ProfileInventoriesTable>(() => ProfileInventoriesTable.Create(DataContext));
            profilesTableLoader = new Lazy<ProfilesTable>(() => ProfilesTable.Create(DataContext));
        }

        internal static MtgaSchema Create(DataContext dataContext)
        {
            return new MtgaSchema(dataContext);
        }
    }
}