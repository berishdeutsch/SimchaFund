using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimchaFund.Web.Models;
using SimchasFund.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFund.Web.Controllers
{
    public class HomeController : Controller

    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;";
        public IActionResult Index()
        {
            var mgr = new SimchaFundMgr(_connectionString);
            return View(new SimchasViewModel
            {
                Simchas = mgr.GetSimchas(),
                ContributorCount = mgr.GetContributors().Count
            });
        }
        [HttpPost]
        public IActionResult New(Simcha simcha)
        {
            var mgr = new SimchaFundMgr(_connectionString);
            mgr.AddSimcha(simcha);
            return Redirect("/");
        }
        public IActionResult Contributions(int simchaId)
        {
            var mgr = new SimchaFundMgr(_connectionString);
            return View(new ContributionsViewModel
            {
                Simcha = mgr.GetSimchaById(simchaId)
            });
        }
    }
}