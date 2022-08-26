using MindSculptor.App.AppDataContext.Schemas;
using MindSculptor.DataAccess.Context;
using System;
using System.Data.Common;

namespace MindSculptor.App.AppDataContext
{
    public class AppDataContext : DatabaseContext
    {
        private Lazy<CardsSchema> cardsSchemaLoader;
        private Lazy<MtgaSchema> mtgaSchemaLoader;

        public CardsSchema Cards => cardsSchemaLoader.Value;

        public MtgaSchema Mtga => mtgaSchemaLoader.Value;

        private AppDataContext(string connectionString) : base(connectionString)
        {
            cardsSchemaLoader = new Lazy<CardsSchema>(() => CardsSchema.Create(this));
            mtgaSchemaLoader = new Lazy<MtgaSchema>(() => MtgaSchema.Create(this));
        }

        public static AppDataContext Create(string connectionString)
        {
            return new AppDataContext(connectionString);
        }
    }
}