using Microsoft.AspNetCore.Mvc;
using SimchaFund.Web.Models;
using SimchasFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFund.Web.Controllers
{
    public class ContributorsController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;";
        public IActionResult Index()
        {
            var mgr = new SimchaFundMgr(_connectionString);
            return View(new ContributorsViewModel
            {
                Contributors = mgr.GetContributors()
            });
        }
        [HttpPost]
        public IActionResult New(Contributor contributor)
        {
            var mgr = new SimchaFundMgr(_connectionString);
            mgr.AddContributor(contributor);
            return Redirect("/contributors");
        }

        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            var mgr = new SimchaFundMgr(_connectionString);
            mgr.AddDeposit(deposit);
            return Redirect("/contributors");
        }
        public IActionResult History()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Update(Contributor contributor)
        {
            var mgr = new SimchaFundMgr(_connectionString);
            mgr.UpdateContributor(contributor);
            return Redirect("/contributors");
        }
    }
}
