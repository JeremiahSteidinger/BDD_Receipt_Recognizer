using System;
using System.Xml.Linq;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Storage.Queues;
using BDD_Receipt_Recognizer.Data.MongoDb;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

[assembly: FunctionsStartup(typeof(BDD_Receipt_Recognizer.Startup))]

namespace BDD_Receipt_Recognizer
{
	public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            #region MongDb
            MongoDbSettings mongoDbSettings = new MongoDbSettings()
            {
                ConnectionString = Environment.GetEnvironmentVariable("MongoDBConnectionString"),
                DatabaseName = Environment.GetEnvironmentVariable("MongoDBCDatabaseName")
            };
            builder.Services.AddSingleton(mongoDbSettings);
            builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            #endregion

            #region Azure Form Recognizer Client
            string endpoint = Environment.GetEnvironmentVariable("FormRecongizerEndpoint");
            string key = Environment.GetEnvironmentVariable("FormRecongizerKey");
            AzureKeyCredential credential = new AzureKeyCredential(key);

            builder.Services.AddSingleton(new DocumentAnalysisClient(new Uri(endpoint), credential));
            #endregion

            #region Queue Storage
            string queueStorageConnectionString = Environment.GetEnvironmentVariable("QueueStorageConnectionString");
            string queueStraogeQueueName = Environment.GetEnvironmentVariable("QueueStorageQueueName");
            builder.Services.AddSingleton(new QueueClient(queueStorageConnectionString, queueStraogeQueueName));
            #endregion
        }
    }
}

