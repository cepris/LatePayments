using System;
using System.Globalization;

namespace LatePaymentsEu
{
    public class LatePaymentsEu
    {
        public float CalculateInterest(RegionInfo region, DateTime from, DateTime to, float debt)
        {
            IInterestCalculator calculator = InterestCalculatorFactory.GetCalculator(region.Name);
            return 0;
        }
    }
}
