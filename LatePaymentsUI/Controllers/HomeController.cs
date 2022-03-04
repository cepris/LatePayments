using LatePaymentsEu;
using LatePaymentsUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LatePaymentsUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index(InterestModel interestInput)
        {
            if (interestInput.From == DateTime.MinValue)
            {
                interestInput.From = new DateTime(2021, 1, 1);
            }

            if (interestInput.To == DateTime.MinValue)
            {
                interestInput.To = DateTime.Now;
            }

            return View(interestInput);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculate([Bind] InterestModel interestInput)
        {
            if (interestInput.From > interestInput.To)
            {
                ModelState.AddModelError("To", "To date cannot be before From date.");
            }
            if (interestInput.Amount == 0 || interestInput.Amount < 0)
            {
                ModelState.AddModelError("Amount", "Amount has to be greater than 0.");
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("Calculation", interestInput);
            }
            else
            {
                return View("Index", interestInput);
            }
        }

        public IActionResult Calculation(InterestModel interestInput)
        {
            IInterestCalculator calc = InterestCalculatorFactory.GetCalculator(interestInput.Country);
            if (calc is null)
            {
                return View("Index", interestInput);
            }
            
            List<Transaction> trans = new List<Transaction>();
            trans.Add(new Transaction(interestInput.From, interestInput.Amount));
            InterestReport report = calc.GetInterest(interestInput.To, trans);

            //InterestReport report = new InterestReport();

            return View(report);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
