using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivMusic.Models
{
    public class Vacation
    {
        public int id { get; set; }
        public DateTime startvacationdate { get; set; }
        public DateTime endvacationdate { get; set; }
        public int employeeid { get; set; }
        public int hrmanagerid { get; set; }
        public int vacationtypeid { get; set; }
    }
}
