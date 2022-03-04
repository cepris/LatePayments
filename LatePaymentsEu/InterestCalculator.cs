using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatePaymentsEu
{
    public abstract class InterestCalculator : IInterestCalculator
    {

        public readonly InterestReport Interest = new InterestReport();

		protected abstract InterestReport GetInterest(DateTime from, DateTime to, decimal debt);

        public InterestReport AddLoan(decimal debt, DateTime date)
        {
            InterestReportItem newItem = new InterestReportItem();
            newItem.From = date;
            newItem.Amount = debt;
            newItem.Type = TransactionType.Loan;
            Interest.ReportItems.Add(newItem);
            return Interest;
        }

        public InterestReport AddPayment(decimal debt, DateTime date)
        {
            InterestReportItem newItem = new InterestReportItem();
            newItem.From = date;
            newItem.Amount = debt;
            newItem.Type = TransactionType.Payment;
            Interest.ReportItems.Add(newItem);
            return Interest;
        }

        public InterestReport AddCosts(decimal debt, DateTime date)
        {
            InterestReportItem newItem = new InterestReportItem();
            newItem.From = date;
            newItem.Amount = debt;
            newItem.Type = TransactionType.Costs;
            Interest.ReportItems.Add(newItem);
            return Interest;
        }

        public InterestReport GetInterest(DateTime to, List<Transaction> transactions)
        {
            transactions = transactions.OrderBy(s => s.TransactionDate).ToList();
            decimal debt = 0;
            for (int i=0; i<transactions.Count; i++)
            {

                //We do not calculate interest for the costs, therefore we just add them and continue with the next transaction
                if (transactions[i].Type == TransactionType.Costs)
                {
                    AddCosts(transactions[i].TransactionAmount, transactions[i].TransactionDate);
                    continue;
                }

                //If this is a loan, we need to increase debt
                if (transactions[i].Type == TransactionType.Loan)
                {
                    debt += transactions[i].TransactionAmount;
                    AddLoan(transactions[i].TransactionAmount, transactions[i].TransactionDate);
                }
                
                //With every iteration we assume that we can calculate to the end date
                DateTime toDate = to;

                //Includes transactions until one before end to check if we need
                //to split calculation
                if (i < transactions.Count-1)
                {
                    //If the next transaction took place before the end date, we need to calculate interest
                    //to the date of next transaction only
                    if (transactions[i+1].TransactionDate < toDate)
                    {
                        toDate = transactions[i + 1].TransactionDate;
                    }   
                }

                //Calculate interest for time interval between transactions or to the end date
                GetInterest(transactions[i].TransactionDate.AddDays(1), toDate, debt);

                //If this is a payment, we need to decrease debt
                if (transactions[i].Type == TransactionType.Payment)
                {
                    debt -= transactions[i].TransactionAmount;
                    AddPayment(transactions[i].TransactionAmount, transactions[i].TransactionDate);
                }

                //If there were no further transactions, we just calculated to the end date
                //and we are done
                if (toDate == to)
                {
                    break;
                }

            }
            return Interest;
        }
    }
}
