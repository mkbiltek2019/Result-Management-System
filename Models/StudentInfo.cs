using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class StudentInfo
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public int StudentId { get; set; }
        public int Reg { get; set; }
        public string Faculty { get; set; }
        public string Session { get; set; }
        public string Regularity { get; set; }
        public string Hall { get; set; }
        public string Blood { get; set; }
        public string Sex { get; set; }
        public string Fathers_name { get; set; }
        public string Mothers_name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string Religion { get; set; }

        public static List<StudentInfo> GetStudentInfo(List<CurrentSemester> CurrentSemester)
        {
            if (CurrentSemester == null) return null;

            List<StudentInfo> AllStudents = new List<StudentInfo>();

            ProjectDB db = new ProjectDB();
            foreach (CurrentSemester cs in CurrentSemester)
            {
                StudentInfo Student = db.StudentInfo.Where(si => si.StudentId == cs.StudentId).FirstOrDefault();
                if (Student != null) AllStudents.Add(Student);
            }

            return AllStudents;
        }

    }
}