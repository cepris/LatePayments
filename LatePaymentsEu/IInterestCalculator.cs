using System;
using System.Collections.Generic;
using System.Text;

namespace LatePaymentsEu
{
    public interface IInterestCalculator
    {        
        InterestReport GetInterest(DateTime to, List<Transaction> transactions);
    }
}
