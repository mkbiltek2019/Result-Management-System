using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class GetEnrollment
    {
        SqlConnection myConn = new SqlConnection();
        public GetEnrollment(string Faculty)
        {
            myConn = new SqlConnectionGenerator().FromFaculty(Faculty);

        }

        public List<EnrolledStudent> GetStudentEnrollment(string course_code, int semester)
        {
            List<EnrolledStudent> studentList = new List<EnrolledStudent>();

            string sql = "SELECT  XX.StudentId,XX.RegNo,XX.Name,Mid,Attendence,Assignment FROM(SELECT CurrentSemester.StudentId, RegNo,Name FROM (";
            if (semester % 2 == 0)
            {
                sql += "JulEnrollment INNER JOIN CurrentSemester ON JulEnrollment.StudentId = CurrentSemester.StudentId and  JulEnrollment.Semester = CurrentSemester.Semester)";
            }
            else
            {
                sql += "JanEnrollment INNER JOIN CurrentSemester ON JanEnrollment.StudentId = CurrentSemester.StudentId and  JanEnrollment.Semester = CurrentSemester.Semester)";

            }
            sql += " WHERE " + course_code + "='true' )XX LEFT JOIN " + course_code + " ON XX.StudentId=" + course_code + ".StudentId and XX.RegNo=" + course_code + ".RegNo ";

            SqlCommand myCommand = new SqlCommand(sql, myConn);

            try
            {
                myConn.Open();
                SqlDataReader oReader = myCommand.ExecuteReader();

                while (oReader.Read())
                {
                    EnrolledStudent Ens = new EnrolledStudent();
                    Ens.StudentId=Convert.ToInt32(oReader["StudentId"]);
                    Ens.RegNo=  Convert.ToInt32(oReader["RegNo"]) ;
                    Ens.Name=Convert.ToString(oReader["Name"]);
                    try { Ens.Mid = Convert.ToDouble(oReader["Mid"]); }
                    catch (InvalidCastException ex) { Ens.Mid = 1000; }
                    try { Ens.Attendence = Convert.ToDouble(oReader["Attendence"]); }
                    catch (InvalidCastException ex) { Ens.Attendence = 1000; }
                    try { Ens.Assignment = Convert.ToDouble(oReader["Assignment"]); }
                    catch (InvalidCastException ex) { Ens.Assignment = 1000; }

                   
                    studentList.Add( Ens);
                }
            }
            catch ( SqlException ex)
            {
                studentList = null;
            }  
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }


            return studentList;
        }

        public static List<Course> GetEnrollCourses(string Faculty, string Session, int Semester)
        {
            ProjectDB db = new ProjectDB();
            List<Course> Courses = new List<Course>();

            string tableName = (Semester % 2 == 0) ? "JulEnrollment" : "JanEnrollment";
            SqlConnection myConn = new SqlConnectionGenerator().FromFaculty(Faculty);
            string SQLCmd = "select column_name from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + tableName + "'";
            // * can be column_name, data_type, column_default, is_nullable
            SqlCommand myCommand = new SqlCommand(SQLCmd, myConn);
            try
            {
                myConn.Open();
                SqlDataReader oReader = myCommand.ExecuteReader();
                while (oReader.Read())
                {
                    string key = Convert.ToString(oReader.GetValue(0));
                    if (!(key.Equals("id") || key.Equals("Name") || key.Equals("StudentId") || key.Equals("RegNo") || key.Equals("Session") || key.Equals("Semester")))
                    {
                        Course Course = db.Courses.Where(c => c.Course_code == key).FirstOrDefault();
                        if (Course != null)
                        {
                            int ObSem = Course.GetSemesterFromCourse(Course.Course_code);
                            if(ObSem == Semester) Courses.Add(Course);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                
            }
            finally
            {
                if (myConn.State == ConnectionState.Open) myConn.Close();
            }

            return Courses;
        }

        public  List<int> GetCurrentStudentBySemester(int Semester)
        {
           
            List<int> StudentList = new List<int>();

             string SQLCmd = "select StudentId from CurrentSemester where Semester='" + Semester + "'";
            // * can be column_name, data_type, column_default, is_nullable
            SqlCommand myCommand = new SqlCommand(SQLCmd, myConn);
            try
            {
                myConn.Open();
                SqlDataReader oReader = myCommand.ExecuteReader();
               
                while (oReader.Read())
                {
                    StudentList.Add(Convert.ToInt32(oReader.GetValue(0)));

                    
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                if (myConn.State == ConnectionState.Open) myConn.Close();
            }

            return StudentList;
        }
    }
}