using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickFind.Areas.IowaOnline.Models
{
    public class IowaOnlineChild
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string LastContact { get; set; }
        public string AgeThen { get; set; }
        public string AgeNow { get; set; }
        public string OriginatingAgency { get; set; }
        public string IncidentType { get; set; }
        public string Description { get; set; }
    }
}
