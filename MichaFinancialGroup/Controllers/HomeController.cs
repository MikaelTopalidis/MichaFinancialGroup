using MichaFinancialGroup.Models;
using MichaFinancialGroup.Services;
using MichaFinancialGroup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace MichaFinancialGroup.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStatistics _statistics;

        public HomeController(ILogger<HomeController> logger, IStatistics statistics)
        {
            _logger = logger;
            _statistics = statistics;
        }

        //Du ska se antal kunder, antal konton och summan av saldot på konton.

        [ResponseCache(Duration = 30)]
        public IActionResult Index()
        {
            var viewModel = new HomeIndexViewModel();
            viewModel.Statistics = _statistics.GetAll().ToLookup(c => c.Customer.Country).Select(s => new HomeIndexViewModel.StatisticsPerCountryViewModel
            {
                Country = s.Key,
                Customers = s.Select(c => c.CustomerId).Distinct().Count(),
                Accounts = s.Where(a => a.Type == "OWNER").Select(a => a.AccountId).Distinct().Count(),
                TotalBalance = s.Where(a => a.Type == "OWNER").Select(b => b.Account.Balance).Sum()

            }).OrderByDescending(b => b.TotalBalance).ToList();

            viewModel.TotalBalance = _statistics.GetAll().Where(a => a.Type == "OWNER").Select(b => b.Account.Balance).Sum();
            viewModel.Accounts = _statistics.GetAll().Where(a => a.Type == "OWNER").Select(a => a.AccountId).Distinct().Count();
            viewModel.Customers = _statistics.GetAll().Select(c => c.CustomerId).Distinct().Count();
            viewModel.LargestAccount = _statistics.GetAll().OrderByDescending(b => b.Account.Balance).Select(t => t.Account.Balance).FirstOrDefault();

            return View(viewModel);
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
