using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchasFund.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string SimchaName { get; set; }   
        public int SimchaContributors { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
    }
}
