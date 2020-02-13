﻿using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Cards.Tables
{
    public class RarityTypes : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Cards;


        public static readonly IdField Id = new IdField.Definition
        {
            IsAutoGenerated = true,
            IsReadOnly = true
        };

        public static readonly TextField Value = new TextField.Definition
        {
            MinimumLength = 1,
            MaximumLength = 16,
            IsReadOnly = true
        };

        public static readonly PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = Id
        };

        public static readonly UniqueKey UQ = new UniqueKey.Definition
        {
            Fields = Value
        };
    }
}
