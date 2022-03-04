using System;
using System.Collections.Generic;
using System.Text;

namespace LatePaymentsEu
{
    public class InterestReportItem
    {
		protected DateTime from;

		public DateTime From
		{
			get { return from; }
			set { from = value.Date; } //Use date only, without time
		}

		protected DateTime to;

		public DateTime To
		{
			get { return to; }
			set { to = value.Date; } //Use date only, without time
		}

		private decimal amount;
		public decimal Amount
		{
			get
            {
				if (Type == TransactionType.Payment)
                {
					return -amount;
                }
				else
                {
					return amount;
                }
            }
			set
            {
				amount = value;
            }
		}

		public TransactionType Type
		{
			get;
			set;
		}

		public decimal InterestRate { get; set; }

        public int NumberOfDays { get; set; }

        public InterestLegalGround InterestLegalGround { get; set; }
    }
}
