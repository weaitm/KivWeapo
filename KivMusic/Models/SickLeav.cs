using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivMusic.Models
{
    public class SickLeav
    {
        public int id { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int employeeid { get; set; }
        public int hrmanagerid { get; set; }
        public int sicktypeid { get; set; }
    }
}
