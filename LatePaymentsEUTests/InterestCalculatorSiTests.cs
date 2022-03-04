using LatePaymentsEu;
using System;
using System.Collections.Generic;
using Xunit;

namespace LatePaymentsEUTests
{
    public class InterestCalculatorSiTests
    {
        protected List<Transaction> CreateTransactionList(int day, int month, int year, decimal amount)
        {
            List<Transaction> trans = new List<Transaction>();
            trans.Add(new Transaction(new DateTime(year, month, day), amount));
            return trans;
        }

        [Fact]
        public void GetSimpleInterestTest()
        {
            IInterestCalculator calc = InterestCalculatorFactory.GetCalculator("si");
            InterestReport interest = calc.GetInterest(new DateTime(2022, 2, 25), CreateTransactionList(2, 1, 2019, 100));
            Assert.Equal(25.18M, interest.Interest);
        }

        [Fact]
        public void GetSimpleInterestTodayTest()
        {
            //Just testing for infinite loops if latest interest data is missing
            IInterestCalculator calc = InterestCalculatorFactory.GetCalculator("si");
            InterestReport interest = calc.GetInterest(DateTime.Today, CreateTransactionList(2, 1, 2019, 100));
        }

        [Fact]
        public void GetInterestOverMonthsTest()
        {
            IInterestCalculator calc = InterestCalculatorFactory.GetCalculator("si");
            InterestReport interest = calc.GetInterest(new DateTime(2019, 8, 27), CreateTransactionList(2, 1, 2019, 100));
            Assert.Equal(5.19M, interest.Interest);
            Assert.Equal(100M, interest.Debt);
            Assert.Equal(105.19M, interest.FullDebt);
        }

        [Fact]
        public void GetInterestOverYearsTest()
        {
            //Za te termine sodišèe narobe zaokroži konèno vsoto dolgovanega zneska
            //Napaèno zaokrožen znesek je bil 62.16 - sodišèe je izraèunalo veè!
            IInterestCalculator calc = InterestCalculatorFactory.GetCalculator("si");
            InterestReport interest = calc.GetInterest(new DateTime(2020, 8, 27), CreateTransactionList(31, 12, 2012, 100));
            Assert.Equal(62.14M, interest.Interest);
        }

        [Fact]
        public void GetSimpleTransactionInterestTest()
        {   
            //Sodišèe raèuna tako, da najprej poplaèa obresti, šele nato glavnico
            IInterestCalculator calc = InterestCalculatorFactory.GetCalculator("si");
            List<Transaction> list = new List<Transaction>();
            list.Add(new Transaction (new DateTime(2012, 12, 31),  100, TransactionType.Loan));
            list.Add(new Transaction (new DateTime(2013, 1, 15), 50, TransactionType.Loan));
            list.Add(new Transaction (new DateTime(2013, 1, 30), 70, TransactionType.Loan ));
            InterestReport interest = calc.GetInterest(new DateTime(2013, 2, 28), list);
            Assert.Equal(2.43M, interest.Interest);
        }

        [Fact]
        public void GetSimpleLongTermTransactionInterestTest()
        {
            //Preprost izraèun, vendar èez daljše obdobje
            IInterestCalculator calc = InterestCalculatorFactory.GetCalculator("si");
            List<Transaction> list = new List<Transaction>();
            list.Add(new Transaction (new DateTime(2012, 12, 31), 100, TransactionType.Loan ));
            list.Add(new Transaction (new DateTime(2014, 1, 15), 50, TransactionType.Loan ));
            list.Add(new Transaction (new DateTime(2015, 1, 30), 70, TransactionType.Loan ));
            InterestReport interest = calc.GetInterest(new DateTime(2017, 2, 28), list);
            Assert.Equal(58.50M, interest.Interest);
        }

        [Fact]
        public void GetComplexTransactionInterestTest()
        {
            //Sodišèe raèuna tako, da najprej poplaèa obresti, šele nato glavnico
            IInterestCalculator calc = InterestCalculatorFactory.GetCalculator("si");
            List<Transaction> list = new List<Transaction>();
            list.Add(new Transaction (new DateTime(2012, 12, 31), 100, TransactionType.Loan));
            list.Add(new Transaction (new DateTime(2013, 1, 15), 20, TransactionType.Payment));
            list.Add(new Transaction (new DateTime(2013, 1, 30), 20, TransactionType.Costs));
            InterestReport interest = calc.GetInterest(new DateTime(2013, 2, 28), list);
            Assert.Equal(0.85M, interest.Interest);
        }

    }
}
