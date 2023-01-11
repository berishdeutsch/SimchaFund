using SimchasFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFund.Web.Models
{
    public class SimchasViewModel
    {
        public List<Simcha> Simchas { get; set; }
        public int ContributorCount { get; set; }
    }
}
