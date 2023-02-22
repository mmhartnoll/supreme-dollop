using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables;
using MindSculptor.DataAccess.Context;
using System;

namespace MindSculptor.App.AppDataContext.Schemas
{
    public class MtgaSchema : DatabaseSchema
    {
        private Lazy<ActiveEventEntriesTable> activeEventEntriesTableLoader;
        private Lazy<BoostersTable> boostersTableLoader;
        private Lazy<DigitalCardsTable> digitalCardsTableLoader;
        private Lazy<DraftEventsTable> draftEventsTableLoader;
        private Lazy<DraftPickOptionsTable> draftPickOptionsTableLoader;
        private Lazy<DraftPicksTable> draftPicksTableLoader;
        private Lazy<EventEntriesTable> eventEntriesTableLoader;
        private Lazy<EventMatchesTable> eventMatchesTableLoader;
        private Lazy<EventMatchGamesTable> eventMatchGamesTableLoader;
        private Lazy<EventMatchResultsTable> eventMatchResultsTableLoader;
        private Lazy<EventsTable> eventsTableLoader;
        private Lazy<PlayersTable> playersTableLoader;
        private Lazy<ProfileHasBoostersTable> profileHasBoostersTableLoader;
        private Lazy<ProfileHasCardsTable> profileHasCardsTableLoader;
        private Lazy<ProfileInventoriesTable> profileInventoriesTableLoader;
        private Lazy<ProfilesTable> profilesTableLoader;

        public ActiveEventEntriesTable ActiveEventEntries => activeEventEntriesTableLoader.Value;

        public BoostersTable Boosters => boostersTableLoader.Value;

        public DigitalCardsTable DigitalCards => digitalCardsTableLoader.Value;

        public DraftEventsTable DraftEvents => draftEventsTableLoader.Value;

        public DraftPickOptionsTable DraftPickOptions => draftPickOptionsTableLoader.Value;

        public DraftPicksTable DraftPicks => draftPicksTableLoader.Value;

        public EventEntriesTable EventEntries => eventEntriesTableLoader.Value;

        public EventMatchesTable EventMatches => eventMatchesTableLoader.Value;

        public EventMatchGamesTable EventMatchGames => eventMatchGamesTableLoader.Value;

        public EventMatchResultsTable EventMatchResults => eventMatchResultsTableLoader.Value;

        public EventsTable Events => eventsTableLoader.Value;

        public PlayersTable Players => playersTableLoader.Value;

        public ProfileHasBoostersTable ProfileHasBoosters => profileHasBoostersTableLoader.Value;

        public ProfileHasCardsTable ProfileHasCards => profileHasCardsTableLoader.Value;

        public ProfileInventoriesTable ProfileInventories => profileInventoriesTableLoader.Value;

        public ProfilesTable Profiles => profilesTableLoader.Value;

        private MtgaSchema(DatabaseContext databaseContext) : base(databaseContext)
        {
            activeEventEntriesTableLoader = new Lazy<ActiveEventEntriesTable>(() => ActiveEventEntriesTable.Create(databaseContext));
            boostersTableLoader = new Lazy<BoostersTable>(() => BoostersTable.Create(databaseContext));
            digitalCardsTableLoader = new Lazy<DigitalCardsTable>(() => DigitalCardsTable.Create(databaseContext));
            draftEventsTableLoader = new Lazy<DraftEventsTable>(() => DraftEventsTable.Create(databaseContext));
            draftPickOptionsTableLoader = new Lazy<DraftPickOptionsTable>(() => DraftPickOptionsTable.Create(databaseContext));
            draftPicksTableLoader = new Lazy<DraftPicksTable>(() => DraftPicksTable.Create(databaseContext));
            eventEntriesTableLoader = new Lazy<EventEntriesTable>(() => EventEntriesTable.Create(databaseContext));
            eventMatchesTableLoader = new Lazy<EventMatchesTable>(() => EventMatchesTable.Create(databaseContext));
            eventMatchGamesTableLoader = new Lazy<EventMatchGamesTable>(() => EventMatchGamesTable.Create(databaseContext));
            eventMatchResultsTableLoader = new Lazy<EventMatchResultsTable>(() => EventMatchResultsTable.Create(databaseContext));
            eventsTableLoader = new Lazy<EventsTable>(() => EventsTable.Create(databaseContext));
            playersTableLoader = new Lazy<PlayersTable>(() => PlayersTable.Create(databaseContext));
            profileHasBoostersTableLoader = new Lazy<ProfileHasBoostersTable>(() => ProfileHasBoostersTable.Create(databaseContext));
            profileHasCardsTableLoader = new Lazy<ProfileHasCardsTable>(() => ProfileHasCardsTable.Create(databaseContext));
            profileInventoriesTableLoader = new Lazy<ProfileInventoriesTable>(() => ProfileInventoriesTable.Create(databaseContext));
            profilesTableLoader = new Lazy<ProfilesTable>(() => ProfilesTable.Create(databaseContext));
        }

        internal static MtgaSchema Create(DatabaseContext databaseContext)
        {
            return new MtgaSchema(databaseContext);
        }
    }
}