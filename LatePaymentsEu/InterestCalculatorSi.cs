using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace LatePaymentsEu
{
    class InterestCalculatorSi : InterestCalculator
    {
        /*
         * Povzetek ureditve zamudnih obrestnih mer skozi zgodovino je objavljen na
         * spletni strani Banke Slovenije: https://www.bsi.si/statistika/obrestne-mere/temeljna-in-zamudna-obrestna-mera 
         */

        public List<InterestLegalGround> interestLegalGroundList;

        public List<LegalGround> legalGroundList;

        public InterestCalculatorSi()
        {
            //Read from embedded file with interest rates
            ManifestEmbeddedFileProvider manifestEmbeddedProvider = new ManifestEmbeddedFileProvider(Assembly.GetExecutingAssembly());
            IFileInfo info = manifestEmbeddedProvider.GetFileInfo("InterestData/SiInterestrates.json");
            using (Stream stream = info.CreateReadStream())
            {
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                var utf8Reader = new Utf8JsonReader(ms.ToArray());
                interestLegalGroundList = JsonSerializer.Deserialize<List<InterestLegalGround>>(ref utf8Reader);

                //Sort interest rates by date ascending
                interestLegalGroundList.Sort((ground1, ground2) => ground1.ValidFrom.CompareTo(ground2.ValidFrom));
            };

            //Read general legal grounds from embedded resource
            manifestEmbeddedProvider = new ManifestEmbeddedFileProvider(Assembly.GetExecutingAssembly());
            info = manifestEmbeddedProvider.GetFileInfo("InterestData/SiLegalGrounds.json");

            using (Stream stream = info.CreateReadStream())
            {
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                var utf8Reader = new Utf8JsonReader(ms.ToArray());
                legalGroundList = JsonSerializer.Deserialize<List<LegalGround>>(ref utf8Reader);

                //Sort legal grounds by date ascending
                legalGroundList.Sort((ground1, ground2) => ground1.ValidFrom.CompareTo(ground2.ValidFrom));
            };
        }

        protected override InterestReport GetInterest(DateTime from, DateTime to, decimal debt)
        {
            InterestReportItem reportItem = new InterestReportItem();

            //Calculating from the start date
            reportItem.From = from;                        
            
            //Use requested transaction type
            reportItem.Type = TransactionType.Interest;

            //Lookup legal interest rate valid at the start of the interval (from date)
            var groundRate = interestLegalGroundList.Where(s => s.ValidFrom <= from).OrderByDescending(s => s.ValidFrom).First();

            //Save reference to interest rate for reporting purposes
            reportItem.InterestLegalGround = groundRate;

            decimal yearlyInterestRate = groundRate.InterestRate;

            //Check if the validity of current interest rate is shorter than target to date. In that case
            //use shorter date.
            reportItem.To = to > groundRate.ValidTo ? groundRate.ValidTo : to;

            //Calculate interval in days for this calculation
            //Add one as the first day is now already day one, not zero
            int interval = (reportItem.To - reportItem.From).Days + 1;

            reportItem.NumberOfDays = interval;

            //Set number of days in a year to allow calculation of interest rate per day
            int daysInYear = DateTime.IsLeapYear(from.Year) ? 366 : 365;

            //Set interest rate of the calculation period
            reportItem.InterestRate = groundRate.InterestRate;

            //Calculate interest for determined time period and round to two decimals
            decimal result = debt * ((decimal)interval / (decimal)daysInYear) * (yearlyInterestRate /  100);
            //reportItem.Interest = Math.Round(result, 2);
            reportItem.Amount = Math.Round(result, 2);

            Interest.ReportItems.Add(reportItem);

            if (reportItem.To < to)
            {
                GetInterest(reportItem.To.AddDays(1), to, debt);
            }

            return this.Interest;
        }
    }
}
