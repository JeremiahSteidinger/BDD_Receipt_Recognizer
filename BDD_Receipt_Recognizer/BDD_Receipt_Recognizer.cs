using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Storage.Queues;
using BDD_Receipt_Recognizer.Data.MongoDb;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BDD_Receipt_Recognizer
{
    public class BDD_Receipt_Recognizer
    {
        private DocumentAnalysisClient _documentAnalysisClient;
        private readonly IMongoRepository<Receipt> _receiptRepository;
        private QueueClient _queueClient;

        public BDD_Receipt_Recognizer(DocumentAnalysisClient documentClient, IMongoRepository<Receipt> receiptRepository, QueueClient queueClient)
        {
            _documentAnalysisClient = documentClient;
            _receiptRepository = receiptRepository;
            _queueClient = queueClient;
        }

        [FunctionName("BDD_Receipt_Recognizer")]
        [StorageAccount("QueueStorageConnectionString")]
        public async Task Run([BlobTrigger("receipts/{name}")] Stream receiptBlob, string name, ILogger log)
        {
            log.LogDebug($"Function running for blob: {name}");

            ReceiptRecognizer receiptRecognizer = new ReceiptRecognizer(_documentAnalysisClient);

            var receipts = await receiptRecognizer.GetReceiptContents(receiptBlob);

            foreach(Receipt receipt in receipts)
            {
                receipt.BlobName = name;

                await _receiptRepository.InsertOneAsync(receipt);

                if (receipt.NeedsManualReview())
                {
                    await _queueClient.SendMessageAsync(receipt.Id.ToString());
                }
            }
        }
    }
}

