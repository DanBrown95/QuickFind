using System;
using System.Collections.Generic;

namespace QuickFind.Areas.FBI.Models
{
    public class Person
    {
        public Person()
        {
            this.BirthDates = new List<DOB>();
        }

        public string Name { get; set; }
        public List<DOB> BirthDates { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Hair { get; set; }
        public string Eyes { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Sex { get; set; }
        public string Race { get; set; }
        public string Nationality { get; set; }
        public string Details { get; set; }
        public string LastLocation { get; set; }
        public string Profile { get; set; }
        public string PosterUrl { get; set; }
        public string ImageUrl {
            get
            {
                var name = this.Name.Replace(' ', '-').ToLower();
                return "https://www.fbi.gov/wanted/kidnap/"+ name +"/@@images/image/";
            }
        }
        public string[] Images { get; set; }
    }

    public class DOB
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
