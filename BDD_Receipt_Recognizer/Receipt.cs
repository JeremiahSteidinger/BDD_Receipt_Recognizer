using System;
using System.Collections.Generic;
using BDD_Receipt_Recognizer.Data.MongoDb;

namespace BDD_Receipt_Recognizer
{
    [BsonCollection("receipts")]
	public class Receipt : Document
	{
        public Merchant Merchant { get; set; }
        public DateTimeOffset? TransactionDate { get; set; }
        public List<Item> Items { get; set; }
        public double? Total { get; set; }
        public string BlobName { get; set; }
        public DateTime? DateProcessed { get; set; } = DateTime.UtcNow;

        public bool NeedsManualReview()
        {
            bool needsManualReview = false;

            if (string.IsNullOrEmpty(Merchant.Name))
            {
                needsManualReview = true;
            }
            else if (TransactionDate == null)
            {
                needsManualReview = true;
            }
            else if (Total == null)
            {
                needsManualReview = true;
            }
            else
            {
                foreach(Item item in Items)
                {
                    if (string.IsNullOrEmpty(item.Description))
                    {
                        needsManualReview = true;
                        break;
                    }
                    if (item.Total == null)
                    {
                        needsManualReview = true;
                        break;
                    }
                }
            }

            return needsManualReview;
        }
    }
}

