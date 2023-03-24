using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KivMusic
{
    public class Profile
    {
        public int id { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string profilelogin { get; set; }
        public string profilepassword  { get; set; }
        public int rolesid { get; set; }
    }
}
