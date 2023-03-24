using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivMusic.Models
{
    public class Product
    {
        public int id { get; set; }
        public string productname { get; set; }
        public decimal productprice { get; set; }
        public int producttypeid { get; set; }
    }
}
