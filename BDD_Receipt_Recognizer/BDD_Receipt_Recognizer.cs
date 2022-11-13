using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
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

        public BDD_Receipt_Recognizer(DocumentAnalysisClient documentClient, IMongoRepository<Receipt> receiptRepository)
        {
            _documentAnalysisClient = documentClient;
            _receiptRepository = receiptRepository;

        }

        [FunctionName("BDD_Receipt_Recognizer")]
        public async Task Run([BlobTrigger("/{name}", Connection = "")] Stream receiptBlob, string name, ILogger log)
        {
            ReceiptRecognizer receiptRecognizer = new ReceiptRecognizer(_documentAnalysisClient);

            var receipts = await receiptRecognizer.GetReceiptContents(receiptBlob);

            foreach(Receipt receipt in receipts)
            {
                await _receiptRepository.InsertOneAsync(receipt);

                if (receipt.NeedsManualReview())
                {

                }
            }
        }
    }
}

