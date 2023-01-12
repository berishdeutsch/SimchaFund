using SimchasFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFund.Web.Models
{
    public class HistoryViewModel
    {
        public Contributor Contributor { get; set; }
        public List<Simcha> Simchas { get; set; }
    }
}
