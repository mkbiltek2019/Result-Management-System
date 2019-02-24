using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class CRUD
    {
        SqlConnection myConn = new SqlConnection();

        public CRUD(String Faculty)
        {
            myConn = new SqlConnectionGenerator().FromFaculty(Faculty);
        
        }

        public Boolean InsertCurrentSemester(IDictionary<string, object> dict)
        {
            String str;
            str = "INSERT CurrentSemester (StudentID, Semester) VALUES ('" + dict["StudentId"] + "', '" + dict["Semester"] + "')";

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


        public Boolean InsertEnrollment(IDictionary<string, object> dict)
        {
            CurrentSemester currentSemester = CheckIsudentIsPresentOrNot(dict);


            if (currentSemester == null)
            {
                InsertCurrentSemester(dict);

                String sql = "";
                if (Convert.ToInt32(dict["Semester"]) % 2 == 0)
                {
                    sql = "Insert Into JulEnrollment (";

                }
                else
                {
                    sql = "Insert Into JanEnrollment (";

                }
                String value = " VALUES (";
                foreach (KeyValuePair<string, object> item in dict)
                {
                    sql += item.Key + ", ";
                    value += "'" + item.Value + "' , ";
                }
                sql = sql.Substring(0, sql.Length - 2);
                value = value.Substring(0, value.Length - 2);
                sql = sql + ") " + value + ")";

                SqlCommand myCommand = new SqlCommand(sql, myConn);
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

            }
            else
            {
                UpdateEnrollment(dict);

            }

            return true;
        }

        public void UpdateEnrollment(IDictionary<string, object> dict)
        {

            String sql = "";
            if (Convert.ToInt32(dict["Semester"]) % 2 == 0)
            {
                sql = "UPDATE   JulEnrollment SET ";

            }
            else
            {
                sql = "UPDATE   JanEnrollment SET ";

            }

            foreach (KeyValuePair<string, object> item in dict)
            {    if(item.Key!="id")
                sql += item.Key + " = '" + item.Value + "', ";

            }
            sql = sql.Substring(0, sql.Length - 2);

            sql = sql + "WHERE StudentId = '" + dict["StudentId"] + "' AND Semester = '" + dict["Semester"] + "'";

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

        public IDictionary<string, object> SelectEnrollment(int Semester, int StudentId)
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();

            String str;
            if(Semester%2==0)
                str = "SELECT * FROM JulEnrollment  WHERE StudentId ='" + StudentId + "' And Semester ='" + Semester + "'";
            else
                str = "SELECT * FROM JanEnrollment  WHERE StudentId ='" + StudentId + "' And Semester ='" + Semester + "'";

            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                SqlDataReader oReader = myCommand.ExecuteReader();
                if (oReader.Read())
                {
                    for(int i = 0; i < oReader.FieldCount ;i++)
                    {
                        dict.Add(oReader.GetName(i),oReader.GetValue(i));
                    }
                   // currentSemester.StudentId = Convert.ToInt32(oReader["StudentId"]);
                   // currentSemester.Semester = Convert.ToInt32(oReader["Semester"]);
                }
                else
                {
                    dict = null;
                }
            }
            catch (System.Exception ex)
            {
               dict = null;
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }

            return dict;
        }

        private CurrentSemester CheckIsudentIsPresentOrNot(IDictionary<string, object> dict)
        {
            CurrentSemester currentSemester = new CurrentSemester();

            String str;
            str = "SELECT * FROM CurrentSemester  WHERE StudentId ='" + dict["StudentId"] + "'";

            SqlCommand myCommand = new SqlCommand(str, myConn);

            try
            {
                myConn.Open();
                SqlDataReader oReader = myCommand.ExecuteReader();
                if (oReader.Read())
                {
                    currentSemester.StudentId = Convert.ToInt32(oReader["StudentId"]);
                    currentSemester.Semester = Convert.ToInt32(oReader["Semester"]);

                }
                else
                {
                    currentSemester = null;
                }

            }
            catch (System.Exception ex)
            {
                return currentSemester;
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }

            return currentSemester;
        }

         
        internal Boolean DeleteEnrollment(int? id, int semester)
        {
                    
            String sql = "";
            if (semester % 2 == 0)
            {
                sql = "DELETE FROM JulEnrollment WHERE StudentId ='"+id+"' And Semester = '"+semester+"'";

            }
            else
            {
                sql =  "DELETE FROM JanEnrollment WHERE StudentId ='"+id+"' And Semester = '"+semester+"'";
                  

            }
            sql += " DELETE FROM CurrentSemester WHERE StudentId ='" + id + "' And Semester = '" + semester + "'";
          
            SqlCommand myCommand = new SqlCommand(sql, myConn);
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


        public Boolean TruncateCurrentSemester(int semester) {
            String sql = "DELETE FROM CurrentSemester WHERE  Semester = '" + semester + "'";
            SqlCommand myCommand = new SqlCommand(sql, myConn);
            try
            {   myConn.Open();
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
    }
}