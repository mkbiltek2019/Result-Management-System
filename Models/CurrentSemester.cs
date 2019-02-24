using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class CurrentSemester
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int StudentId { get; set; }     
        public int Semester { get; set; }

        public static List<CurrentSemester> GetCurrentSemesterStudents(string Faculty, string Session, int Semester)
        {
            int batchId = Convert.ToInt32(Session.Substring(2, 2)); //startIndex, Length
            int startRange = batchId * 100000;
            int endRange = ((batchId + 1) * 100000) - 1;

            List<CurrentSemester> AllStudents = new List<CurrentSemester>();
            SqlConnection con = new SqlConnectionGenerator().FromFaculty(Faculty);
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM CurrentSemester WHERE StudentId BETWEEN " + startRange + " AND " + endRange + " AND Semester=" + Semester + ";", con);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CurrentSemester CS = new CurrentSemester();
                        CS.StudentId = Convert.ToInt32(reader.GetValue(1));
                        CS.Semester = Convert.ToInt32(reader.GetValue(2));
                        AllStudents.Add(CS);
                    }
                    reader.Close();
                }
                else
                {
                    return null;
                }
            }
            con.Close();

            return AllStudents;
        }
    }
}