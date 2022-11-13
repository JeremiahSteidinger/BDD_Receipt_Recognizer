using System;
namespace BDD_Receipt_Recognizer.Data.MongoDb
{
    public interface IMongoDbSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
}

