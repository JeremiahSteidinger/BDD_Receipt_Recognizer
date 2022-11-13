using System;
using MongoDB.Bson;

namespace BDD_Receipt_Recognizer.Data.MongoDb
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}

