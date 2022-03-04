using System;
using System.Collections.Generic;
using System.Text;

namespace LatePaymentsEu
{
    public class Transaction
    {

		private DateTime transactionDate;

		public DateTime TransactionDate
		{
			get { return transactionDate; }
			set { transactionDate = value.Date; }
		}

		private decimal transactionAmount;

		public decimal TransactionAmount
		{
			get { return transactionAmount; }
			set { transactionAmount = value; }
		}

		public TransactionType Type
        {
			get;
			set;
        }


		public Transaction(DateTime date, decimal amount, TransactionType type = TransactionType.Loan)
        {
			this.TransactionDate = date;
			this.TransactionAmount = amount;
			this.Type = type;
        }
	}    
}
