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

        private MtgaSchema(DatabaseContext dataContext) : base(dataContext)
        {
            activeEventEntriesTableLoader = new Lazy<ActiveEventEntriesTable>(() => ActiveEventEntriesTable.Create(Context));
            boostersTableLoader = new Lazy<BoostersTable>(() => BoostersTable.Create(Context));
            digitalCardsTableLoader = new Lazy<DigitalCardsTable>(() => DigitalCardsTable.Create(Context));
            draftEventsTableLoader = new Lazy<DraftEventsTable>(() => DraftEventsTable.Create(Context));
            draftPickOptionsTableLoader = new Lazy<DraftPickOptionsTable>(() => DraftPickOptionsTable.Create(Context));
            draftPicksTableLoader = new Lazy<DraftPicksTable>(() => DraftPicksTable.Create(Context));
            eventEntriesTableLoader = new Lazy<EventEntriesTable>(() => EventEntriesTable.Create(Context));
            eventMatchesTableLoader = new Lazy<EventMatchesTable>(() => EventMatchesTable.Create(Context));
            eventMatchGamesTableLoader = new Lazy<EventMatchGamesTable>(() => EventMatchGamesTable.Create(Context));
            eventMatchResultsTableLoader = new Lazy<EventMatchResultsTable>(() => EventMatchResultsTable.Create(Context));
            eventsTableLoader = new Lazy<EventsTable>(() => EventsTable.Create(Context));
            playersTableLoader = new Lazy<PlayersTable>(() => PlayersTable.Create(Context));
            profileHasBoostersTableLoader = new Lazy<ProfileHasBoostersTable>(() => ProfileHasBoostersTable.Create(Context));
            profileHasCardsTableLoader = new Lazy<ProfileHasCardsTable>(() => ProfileHasCardsTable.Create(Context));
            profileInventoriesTableLoader = new Lazy<ProfileInventoriesTable>(() => ProfileInventoriesTable.Create(Context));
            profilesTableLoader = new Lazy<ProfilesTable>(() => ProfilesTable.Create(Context));
        }

        internal static MtgaSchema Create(DatabaseContext dataContext)
        {
            return new MtgaSchema(dataContext);
        }
    }
}