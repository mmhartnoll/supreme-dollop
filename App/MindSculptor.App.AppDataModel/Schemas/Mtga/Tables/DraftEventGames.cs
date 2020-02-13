﻿using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Mtga.Tables
{
    public class DraftEventGames : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Mtga;


        public static IdField Id = new IdField.Definition
        {
            IsAutoGenerated = true,
            IsReadOnly = true
        };

        public static IdField DraftEventMatchId = new IdField.Definition
        {
            MappedField = DraftEventMatches.Id,
            IsReadOnly = true
        };

        public static IntegerField GameNumber = new IntegerField.Definition
        {
            MinimumValue = 1,
            MaximumValue = 3,
            IsReadOnly = true,
        };

        public static BooleanField PlayedFirst = new BooleanField.Definition
        {
            IsReadOnly = true
        };

        public static BooleanField GameWon = new BooleanField.Definition
        {
            IsReadOnly = true
        };

        public static PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = Id
        };

        public static UniqueKey UQ = new UniqueKey.Definition
        {
            Fields = FieldReference.List(DraftEventMatchId, GameNumber)
        };

        public static ForeignKey FK = new ForeignKey.Definition
        {
            Fields = DraftEventMatchId,
            ReferencedKey = DraftEventMatches.PK
        };
    }
}
