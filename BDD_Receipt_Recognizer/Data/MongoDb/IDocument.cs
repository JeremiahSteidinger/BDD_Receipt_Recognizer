using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDD_Receipt_Recognizer.Data.MongoDb
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        ObjectId Id { get; set; }

        DateTime CreatedAt { get; }
    }
}

