﻿using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.DataAccess.Modelled.Records.Keys;
using MindSculptor.DataAccess.Modelled.Schemas;

namespace MindSculptor.App.AppDataModel.Schemas.Cards.Tables
{
    class FaceHasMainTypes : RecordDefinition
    {
        public override Schema Schema => MindSculptorDataModel.Cards;


        public static readonly IdField FaceId = new IdField.Definition
        {
            MappedField = Faces.Id,
            IsReadOnly = true
        };

        public static readonly IdField MainTypeId = new IdField.Definition
        {
            MappedField = MainTypes.Id,
            IsReadOnly = true
        };

        public static readonly IntegerField Ordinal = new IntegerField.Definition
        {
            MinimumValue = 0,
            MaximumValue = 7
        };

        public static readonly PrimaryKey PK = new PrimaryKey.Definition
        {
            Fields = FieldReference.List(FaceId, MainTypeId)
        };

        public static readonly UniqueKey UQ = new UniqueKey.Definition
        {
            Fields = FieldReference.List(FaceId, Ordinal)
        };

        public static readonly ForeignKey FK1 = new ForeignKey.Definition
        {
            Fields = FaceId,
            ReferencedKey = Faces.PK
        };

        public static readonly ForeignKey FK2 = new ForeignKey.Definition
        {
            Fields = MainTypeId,
            ReferencedKey = MainTypes.PK
        };
    }
}
