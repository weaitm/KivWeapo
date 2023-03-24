using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivMusic.Models
{
    public class Delivery
    {
        public int id { get; set; }
        public int warehouseid { get; set; }
        public int profileid { get; set; }
        public int productid { get; set; }
        public int providerid { get; set; }
    }
}
