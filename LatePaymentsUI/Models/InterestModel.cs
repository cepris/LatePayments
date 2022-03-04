using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LatePaymentsUI.Models
{
    public class InterestModel
    {
        [Display(Name = "From date")]
        [DataType(DataType.Date)]
        public DateTime From
        { get; set; }

        [Display(Name = "To date")]
        [DataType(DataType.Date)]
        public DateTime To
        { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount
        { get; set; }

        [Display(Name = "Country")]
        public string Country
        { get; set;  }
    }
}
