using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BDD.ReceiptRecognizer.Models;

namespace BDD_Receipt_Recognizer
{
	public interface IReceiptRecognizer
	{
		Task<List<Receipt>> GetReceiptContentsAsync(Stream reciptStream);
	}
}

