using System;
using System.Collections.Generic;
using System.Text;

namespace LatePaymentsEu
{
    public class InterestCalculatorFactory
    {
        public static IInterestCalculator GetCalculator(string country)
        {
            switch (country)
            {
                case "si": return new InterestCalculatorSi();
                case "cy": return new InterestCalculatorCy();
                default: return null;
            }
        }
    }
}
