using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivMusic.Models
{
    public class ProductHistory
    {
        public int id { get; set; }
        public string statusrecord { get; set; }
        public string productinfo { get; set; }
        public string characteristiczinfo { get; set; }
        public DateTime dateCreate { get; set; }
    }
}
