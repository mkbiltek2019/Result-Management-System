using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class GradeSheetDataModel
    {

        public int id { get; set; }
        public int SLNo { get; set; }
        public string Name { set; get; }
        public int StudentId { set; get; }
        public int RegNo { set; get; }
        public string Faculty { get; set; }
        public string Session { set; get; }
        public int Semester { get; set; }
        public string Degree { get; set; }

        public List<CourseResult> CourseResults { get; set; }

        public float GPA { get; set; }
        public float PrevCGPA { get; set; }
        public float PrevCCH { get; set; }
        public float CGPA { get; set; }
        public float CCH { get; set; }
        public string Remarks { get; set; }
    }
}