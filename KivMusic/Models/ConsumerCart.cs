using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivMusic.Models
{
    public class ConsumerCart
    {
        public int id { get; set; }
        public int quantityOfProduct { get; set; }

        public int productid { get; set; }
        public int productcheckid { get; set; }

    }
}
