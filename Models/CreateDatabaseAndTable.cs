using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class CreateDatabaseAndTable
    {
        SqlConnection myConn = new SqlConnection();

        public CreateDatabaseAndTable(String Faculty)
        {
            myConn = new SqlConnectionGenerator().FromFaculty(Faculty);
        }

        public void CreateDatabase(String faculty)
        {

            String str;
            SqlConnection Conn = new SqlConnectionGenerator().MasterDatabaseConnection();
            str = "CREATE DATABASE " + faculty;

            SqlCommand myCommand = new SqlCommand(str, Conn);
            try
            {
                Conn.Open();
                myCommand.ExecuteNonQuery();

            }
            catch (System.Exception ex)
            {
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
            CreatejanJulTable(faculty);
        }
        public void CreatejanJulTable(String database)
        {

            String sql = "CREATE TABLE JanEnrollment (id INTEGER  PRIMARY KEY IDENTITY, Name nvarchar(MAX), StudentId int, RegNo int,Session nvarchar(MAX) ,Semester int)"
                        + "CREATE TABLE JulEnrollment (id INTEGER  PRIMARY KEY IDENTITY, Name nvarchar(MAX), StudentId int, RegNo int,Session nvarchar(MAX) ,Semester int)"
                        + "CREATE TABLE CurrentSemester (id INTEGER  PRIMARY KEY IDENTITY, StudentId int ,Semester int)";

            SqlCommand myCommand = new SqlCommand(sql, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }

        }

        public Boolean AlterDatabase(string oldFaculty, String newfaculty)
        {
            String str;
            str = "ALTER DATABASE " + oldFaculty + "  Modify Name = " + newfaculty;

            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();

            }
            catch (System.Exception ex)
            {
                return false;
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
            return true;

        }

        public Boolean AlterCourseTable(string oldCourse_code, String newCourse_code)
        {
            String str;
            str = " EXEC sp_rename " + oldCourse_code + ", " + newCourse_code;

            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();

            }
            catch (System.Exception ex)
            {
                return false;
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
            return true;

        }

        public Boolean RenameTableColumn(string tableName, string oldColumn, string newColumn)
        {
            string str = "EXEC SP_RENAME '" + tableName + "." + oldColumn + "', '" + newColumn + " ', 'COLUMN'";

            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();

            }
            catch (System.Exception ex)
            {
                return false;
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
            return true;

        }

        public String AddCourse(String courseCode, int semester)
        {
            String sql;
            if (semester % 2 == 0)
            {
                sql = "ALTER TABLE JulEnrollment add  [" + courseCode + "] BIT default 'FALSE'";
            }
            else
            {
                sql = "ALTER TABLE JanEnrollment add  [" + courseCode + "] BIT default 'FALSE'";
            }

            SqlCommand myCommand = new SqlCommand(sql, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
            string message = CreateCourseTable(courseCode);
            return message;
        }

        public string CreateCourseTable(String courseCode)
        {
            String sql;


            sql = "CREATE TABLE " + courseCode + " (StudentId INTEGER, RegNo int, Mid float, Attendence float, Assignment float, Final float, Submitted BIT default 'FALSE')";


            SqlCommand myCommand = new SqlCommand(sql, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                myConn.Close();
                return courseCode + "Course Already Exist..";
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();

                }

            }
            return courseCode + " Course Added Successfulley..";
        }



    }
}