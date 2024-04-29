using System;
using System.Collections.Generic;
using System.IO;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Threading.Tasks;
using BDD.ReceiptRecognizer.Models;

namespace BDD_Receipt_Recognizer
{
	public class ReceiptRecognizer : IReceiptRecognizer
	{
		private DocumentAnalysisClient _documentAnalysisClient;

		public ReceiptRecognizer(DocumentAnalysisClient documentAnalysisClient)
		{
			_documentAnalysisClient = documentAnalysisClient;
		}

		public async Task<List<Receipt>> GetReceiptContentsAsync(Stream reciptStream)
		{
            AnalyzeDocumentOperation operation = await _documentAnalysisClient.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-receipt", reciptStream);

            AnalyzeResult receipts = operation.Value;

            List<Receipt> receiptList = new List<Receipt>();

            foreach (AnalyzedDocument receipt in receipts.Documents)
            {
                Receipt receiptModel = new Receipt()
                {
                    Merchant = new Merchant()
                    {
                        Name = GetMerchantName(receipt)
                    },
                    TransactionDate = GetTransactionDate(receipt),
                    Items = GetItems(receipt),
                    Total = GetTotal(receipt)
                };

                receiptList.Add(receiptModel);
            }

            return receiptList;
        }

        private string GetMerchantName(AnalyzedDocument receipt)
        {
            string merchantName = string.Empty;

            if (receipt.Fields.TryGetValue("MerchantName", out DocumentField merchantNameField))
            {
                if (merchantNameField.FieldType == DocumentFieldType.String)
                {
                    merchantName = merchantNameField.Value.AsString();
                }
            }

            return merchantName;
        }

        private DateTimeOffset? GetTransactionDate(AnalyzedDocument receipt)
        {
            DateTimeOffset? transactionDate = null;

            if (receipt.Fields.TryGetValue("TransactionDate", out DocumentField transactionDateField))
            {
                if (transactionDateField.FieldType == DocumentFieldType.Date)
                {
                    transactionDate = transactionDateField.Value.AsDate();
                }
            }

            return transactionDate;
        }

        private double? GetTotal(AnalyzedDocument receipt)
        {
            double? total = null;

            if (receipt.Fields.TryGetValue("Total", out DocumentField totalField))
            {
                if (totalField.FieldType == DocumentFieldType.Double)
                {
                    total = totalField.Value.AsDouble();
                }
            }

            return total;
        }

        private List<Item> GetItems(AnalyzedDocument receipt)
        {
            List<Item> items = new List<Item>();

            if (receipt.Fields.TryGetValue("Items", out DocumentField itemsField))
            {
                if (itemsField.FieldType == DocumentFieldType.List)
                {
                    foreach (DocumentField itemField in itemsField.Value.AsList())
                    {
                        Item item = new Item();

                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                            if (itemFields.TryGetValue("Description", out DocumentField itemDescriptionField))
                            {
                                if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                {
                                    item.Description = itemDescriptionField.Value.AsString();
                                }
                            }

                            if (itemFields.TryGetValue("TotalPrice", out DocumentField itemTotalPriceField))
                            {
                                if (itemTotalPriceField.FieldType == DocumentFieldType.Double)
                                {
                                    item.Total = itemTotalPriceField.Value.AsDouble();
                                }
                            }
                        }

                        items.Add(item);
                    }
                }
            }

            return items;
        }
	}
}

