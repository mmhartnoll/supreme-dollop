using MindSculptor.DataAccess.Modelled;
using MindSculptor.DataAccess.Modelled.Schemas;
using System;

namespace MindSculptor.App.AppDataModel
{
    public class MindSculptorDataModel : DataModel
    {
        //public static Schema Accounts = new Schema.Definition { };

        public static Schema Cards = new Schema.Definition { };

        public static Schema Mtga = new Schema.Definition { };

        public object SelectMany()
        {
            throw new NotImplementedException();
        }
    }
}
