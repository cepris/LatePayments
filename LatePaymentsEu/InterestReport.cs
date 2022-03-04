using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatePaymentsEu
{
    public class InterestReport
    {
        public string Currency = "EUR";

        private List<InterestReportItem> reportItems = new List<InterestReportItem>();

        public List<InterestReportItem> ReportItems
        {
            get
            {
                return reportItems;
            }
        }

        public decimal Interest
            {
            get
            {
                //return ReportItems.Sum(s => s.Interest);
                return (from item in ReportItems where item.Type == TransactionType.Interest select item.Amount).Sum();
            }
        }

        public decimal Debt
        {
            get
            {
                return (from item in ReportItems where item.Type == TransactionType.Loan select item.Amount).Sum();
            }
        }

        public decimal FullDebt
        {
            get
            {
                return Debt + Interest;
            }
        }
    }
}
