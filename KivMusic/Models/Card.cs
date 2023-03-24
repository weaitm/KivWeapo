using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KivMusic.Models
{
    public class Card
    {
        public int id { get; set; }

        public string cardnumber { get; set; }

        public string cardholder { get; set; }

        public DateTime cardexpirydate { get; set; }

        public string cvcccv { get; set; }

        public int typecardid { get; set; }

        public int paymentsystemid { get; set; }

        public int bankid { get; set; }
    }
}
