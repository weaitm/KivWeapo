using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivMusic.Models
{
    public class Salary
    {
        public int id { get; set; }
        public DateTime paydate { get; set; }
        public decimal paysum { get; set; }
        public int employeeid { get; set; }
        public int bookkeeperid { get; set; }
        public int paytypeid { get; set; }
    }
}
