using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class EnrolledStudent
    {
        public int StudentId { get; set; }
        public int RegNo { get; set; }
        public string Name { get; set; }
        public double Mid { set; get; }
        public double Attendence { set; get; }
        public double Assignment { set; get; }

    }
}