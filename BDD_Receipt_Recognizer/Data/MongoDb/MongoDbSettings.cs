using System;
namespace BDD_Receipt_Recognizer.Data.MongoDb
{
    public class MongoDbSettings : IMongoDbSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}

