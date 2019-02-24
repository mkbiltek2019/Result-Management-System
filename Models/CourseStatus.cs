using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class CourseStatus
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
        public string UnderSession { get; set; }
        public Boolean Status { get; set; }

        public static void AddCourseStatus(string CourseCode, string Session, Boolean Status)
        {
            ProjectDB db = new ProjectDB();
            Course Course = db.Courses.Where(dp => dp.Course_code == CourseCode).FirstOrDefault();
            if (Course == null) return;

            CourseStatus OldCourseStatus = db.CourseStatus.Where(dp => dp.Course_code == Course.Course_code && dp.UnderSession == Session).FirstOrDefault();
            if (OldCourseStatus != null) return;

            CourseStatus CourseStatus = new CourseStatus();
            CourseStatus.Course_code = Course.Course_code;
            CourseStatus.Course_title = Course.Course_title;
            CourseStatus.Credit_hour = Course.Credit_hour;
            CourseStatus.Semester = Course.Semester;
            CourseStatus.UnderFaculty = Course.UnderFaculty;
            CourseStatus.UnderDepartment = Course.UnderDepartment;
            CourseStatus.CourseTeacherID = Course.CourseTeacherID;
            CourseStatus.CourseTeacherName = Course.CourseTeacherName;
            CourseStatus.CourseTeacherDepartment = Course.CourseTeacherDepartment;
            CourseStatus.CourseTeacherFaculty = Course.CourseTeacherFaculty;
            CourseStatus.UnderSession = Session;
            CourseStatus.Status = Status;

            db.CourseStatus.Add(CourseStatus);
            db.SaveChanges();
        }

        public static Boolean UpdateCourseStatus(string CourseCode, string Session, Boolean Status)
        {
            ProjectDB db = new ProjectDB();
            CourseStatus CourseStatus = db.CourseStatus.Where(dp => dp.Course_code == CourseCode && dp.UnderSession == Session).FirstOrDefault();

            if (CourseStatus == null) return false;

            CourseStatus.Status = Status;
            db.Entry(CourseStatus).State = EntityState.Modified;
            db.SaveChanges();

            return true;
        }

        public static Boolean IsMarkSubmitted(Course Course, string Session)
        {
            if (Course == null || Session == null) return false;

            ProjectDB db = new ProjectDB();
            CourseStatus CourseStatus = db.CourseStatus.Where(cs => cs.Course_code == Course.Course_code && cs.UnderSession == Session).FirstOrDefault();
            
            if (CourseStatus == null || CourseStatus.Status == false) return false;

            return true;
        }
    }
}