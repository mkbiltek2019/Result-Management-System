using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class Course
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { set; get; }
        public string Course_code { get; set; }
        public string Course_title { get; set; }
        public float Credit_hour { get; set; }
        public int Semester { get; set; }
        public string UnderFaculty { get; set; }
        public string UnderDepartment { get; set; }
        public int CourseTeacherID { get; set; }
        public string CourseTeacherName { get; set; }
        public string CourseTeacherDepartment { get; set; }
        public string CourseTeacherFaculty { get; set; }

        public static int GetSemesterFromCourse(string CourseCode)
        {
            int Sem = -1;

            if(CourseCode == null || CourseCode.Length < 4) return -1;

            string SemCode = CourseCode.Substring(CourseCode.Length-3, 2);

            switch (SemCode)
            {
                case "11":
                    Sem = 1;
                    break;

                case "12":
                    Sem = 2;
                    break;

                case "21":
                    Sem = 3;
                    break;

                case "22":
                    Sem = 4;
                    break;

                case "31":
                    Sem = 5;
                    break;

                case "32":
                    Sem = 6;
                    break;

                case "41":
                    Sem = 7;
                    break;

                case "42":
                    Sem = 8;
                    break;

                case "51":
                    Sem = 9;
                    break;

                default:
                    Sem = -1;
                    break;
            }

            return Sem;
        }

        public static string CheckRemarks(string Faculty, int StudentId, string Remarks)
        {
            if (Faculty != null || StudentId <= 0 || Remarks == null) return "";

            string CheckedRemarks = "";
            string[] Courses = Remarks.Split(',');

            foreach (string CourseCode in Courses)
            {
                Marks mark = new Marks().GetMark(Faculty, CourseCode, StudentId);

                if (mark != null)
                {
                    float TotalMark = 0;

                    if (mark.Mid != -1) TotalMark += mark.Mid;
                    if (mark.Assignment != -1) TotalMark += mark.Assignment;
                    if (mark.Attendence != -1) TotalMark += mark.Attendence;
                    if (mark.Final != -1) TotalMark += mark.Final;

                    if (!Marks.IsPassed(TotalMark)) CheckedRemarks += CourseCode + ",";
                }
            }

            if (CheckedRemarks != "") CheckedRemarks = CheckedRemarks.Substring(0, CheckedRemarks.Length - 1);

            return CheckedRemarks;
        }
    }
}