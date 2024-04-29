﻿using System;
using System.Xml.Linq;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Storage.Queues;
using BDD.Data.MongoDB;
using BDD.ReceiptRecognizer.Models;
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
            IMongoDbSettings mongoDbSettings = new MongoDbSettings()
            {
                ConnectionString = Environment.GetEnvironmentVariable("MongoDBConnectionString"),
                DatabaseName = Environment.GetEnvironmentVariable("MongoDBCDatabaseName")
            };
            builder.Services.AddSingleton(mongoDbSettings);
            builder.Services.AddSingleton<IMongoRepository<Receipt>, MongoRepository<Receipt>>();
            #endregion

            #region Azure Form Recognizer Client
            string endpoint = Environment.GetEnvironmentVariable("FormRecongizerEndpoint");
            string key = Environment.GetEnvironmentVariable("FormRecognizerKey");
            AzureKeyCredential credential = new AzureKeyCredential(key);

            builder.Services.AddSingleton(new DocumentAnalysisClient(new Uri(endpoint), credential));
            builder.Services.AddSingleton<IReceiptRecognizer, ReceiptRecognizer>();
            #endregion

            #region Queue Storage
            string queueStorageConnectionString = Environment.GetEnvironmentVariable("QueueStorageConnectionString");
            string queueStraogeQueueName = Environment.GetEnvironmentVariable("QueueStorageQueueName");
            builder.Services.AddSingleton(new QueueClient(queueStorageConnectionString, queueStraogeQueueName));
            #endregion
        }
    }
}

