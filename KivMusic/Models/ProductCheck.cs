using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivMusic.Models
{
    public class ProductCheck
    {
        public int id { get; set; }
        public string checknumber { get; set; }
        public string shiftnumber { get; set; }
        public DateTime purchasedate { get; set; }
        public decimal totalsum { get; set; }
        public decimal inputsum { get; set; }
        public int shopid { get; set; }
        public int profilecardid { get; set; }
        public int profileid { get; set; }

    }
}
