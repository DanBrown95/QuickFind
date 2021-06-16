using System.Collections.Generic;

namespace QuickFind.Areas.NCMEC.Models
{
    public class NCMECChild
    {
        public int Age { get; set; }
        public string ApproxAge { get; set; }
        public string CaseNumber { get; set; }
        public string CaseType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool HasThumbnail { get; set; }
        public bool InDay { get; set; }
        public bool InMonth { get; set; }
        public bool IsChild { get; set; }
        public string LangId { get; set; }
        public string MissingCity { get; set; }
        public string MissingCountry { get; set; }
        public string MissingCounty { get; set; }
        public string MissingDate { get; set; }
        public string MissingState { get; set; }
        public string OrgName { get; set; }
        public string OrgPrefix { get; set; }
        public string PosterTitle { get; set; }
        public string Race { get; set; }
        public int SeqNumber { get; set; }
        public string ThumbnailUrl { get; set; }

        public string FullImageUrl {
            get
            {
                return (HasThumbnail) ? "https://api.missingkids.org" + ThumbnailUrl : null; 
            } 
        }
    }

    public class APIRoot
    {
        public List<NCMECChild> Persons { get; set; }
    }
}
