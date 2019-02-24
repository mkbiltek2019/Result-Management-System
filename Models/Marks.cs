using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace PRMS.Models
{
    public class Marks
    {
        public int StudentId { set; get; }
        public int RegNo { set; get; }
        public float Mid { set; get; }
        public float Attendence { set; get; }
        public float Assignment { set; get; }
        public float Final { set; get; }
        public Boolean Submitted { set; get; }

        public Marks()
        {

        }

        public string InserMark(string Faculty, string CourseCode, Marks mark)
        {
            try
            {
                SqlConnection con = new SqlConnectionGenerator().FromFaculty(Faculty);
                con.Open();

                int res = -1;
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + CourseCode + " WHERE StudentId=" + mark.StudentId, con);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cmd = new SqlCommand("UPDATE " + CourseCode + " SET Final=" + mark.Final + " WHERE StudentId=@studentId AND Submitted=@submitted;", con);
                        cmd.Parameters.AddWithValue("@studentId", mark.StudentId);
                        cmd.Parameters.AddWithValue("@submitted", false);
                    }
                    else
                    {
                        cmd = new SqlCommand("INSERT INTO " + CourseCode + " VALUES(@StudentId, @RegNo, @Mid, @Attendence, @Assignment, @Final, @Submitted)", con);

                        cmd.Parameters.AddWithValue("@StudentId", mark.StudentId);
                        cmd.Parameters.AddWithValue("@RegNo", mark.RegNo);
                        cmd.Parameters.AddWithValue("@Mid", mark.Mid);
                        cmd.Parameters.AddWithValue("@Attendence", mark.Attendence);
                        cmd.Parameters.AddWithValue("@Assignment", mark.Assignment);
                        cmd.Parameters.AddWithValue("@Final", mark.Final);
                        cmd.Parameters.AddWithValue("@Submitted", false);
                    }
                }

                res = cmd.ExecuteNonQuery();
                con.Close();

                if (res <= 0) return Messasges.InsertionFailed;
            }
            catch (SqlException ex)
            {
                return Messasges.InsertionFailed + " " + ex.ToString();
            }

            return Messasges.DataInsertedSuccessfully;
        }

        public string InserMarkList(List<Marks> Marks, string Faculty, string CourseCode)
        {
            string content = Faculty + "," + CourseCode;

            try
            {
                SqlConnection con = new SqlConnectionGenerator().FromFaculty(Faculty);
                con.Open();

                int res = -1;

                foreach (Marks Mark in Marks)
                {
                    content += "," + Mark.StudentId;
                    SqlCommand cmd = new SqlCommand("SELECT * FROM " + CourseCode + " WHERE StudentId=" + Mark.StudentId, con);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmd = new SqlCommand("UPDATE " + CourseCode + " SET Final=" + Mark.Final + " WHERE StudentId=@studentId AND Submitted=@submitted;", con);
                            cmd.Parameters.AddWithValue("@studentId", Mark.StudentId);
                            cmd.Parameters.AddWithValue("@submitted", false);
                        }
                        else
                        {
                            cmd = new SqlCommand("INSERT INTO " + CourseCode + " VALUES(@StudentId, @RegNo, @Mid, @Attendence, @Assignment, @Final, @Submitted)", con);

                            cmd.Parameters.AddWithValue("@StudentId", Mark.StudentId);
                            cmd.Parameters.AddWithValue("@RegNo", Mark.RegNo);
                            cmd.Parameters.AddWithValue("@Mid", Mark.Mid);
                            cmd.Parameters.AddWithValue("@Attendence", Mark.Attendence);
                            cmd.Parameters.AddWithValue("@Assignment", Mark.Assignment);
                            cmd.Parameters.AddWithValue("@Final", Mark.Final);
                            cmd.Parameters.AddWithValue("@Submitted", false);
                        }
                    }

                    res = cmd.ExecuteNonQuery();
                }
                con.Close();

                if (res <= 0) return Messasges.InsertionFailed;
            }
            catch (SqlException ex)
            {
                return Messasges.InsertionFailed + " " + ex.ToString();
            }

            return content;
        }

        public Marks GetMark(string Faculty, string CoursCode, int StudentId)
        {
            Marks mark = new Marks();

            SqlConnection con = new SqlConnectionGenerator().FromFaculty(Faculty);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM " + CoursCode + " WHERE StudentId=" + StudentId, con);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    mark.StudentId = Convert.ToInt32(reader.GetValue(0));
                    mark.RegNo = Convert.ToInt32(reader.GetValue(1));
                    mark.Mid = Convert.ToSingle(reader.GetValue(2));
                    mark.Attendence = Convert.ToSingle(reader.GetValue(3));
                    mark.Assignment = Convert.ToSingle(reader.GetValue(4));
                    mark.Final = Convert.ToSingle(reader.GetValue(5));
                    mark.Submitted = Convert.ToBoolean(reader.GetValue(6));
                    reader.Close();
                }
                else
                {
                    return null;
                }
            }
            con.Close();

            return mark;
        }

        public List<Marks> GetMarkList(string faculty, string course_code, string session)
        {
            int batchId = Convert.ToInt32(session.Substring(2, 2)); //startIndex, Length
            int startRange = batchId * 100000;
            int endRange = ((batchId + 1) * 100000) - 1;

            SqlConnection con = new SqlConnectionGenerator().FromFaculty(faculty);
            con.Open();

            List<Marks> marks = new List<Marks>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + course_code + " WHERE StudentId BETWEEN " + startRange + " AND " + endRange + ";", con);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Marks mark = new Marks();
                        mark.StudentId = Convert.ToInt32(reader.GetValue(0));
                        mark.RegNo = Convert.ToInt32(reader.GetValue(1));
                        mark.Mid = Convert.ToSingle(reader.GetValue(2));
                        mark.Attendence = Convert.ToSingle(reader.GetValue(3));
                        mark.Assignment = Convert.ToSingle(reader.GetValue(4));
                        mark.Final = Convert.ToSingle(reader.GetValue(5));
                        mark.Submitted = Convert.ToBoolean(reader.GetValue(6));
                        marks.Add(mark);
                    }
                    reader.Close();
                }
                else
                {
                    return null;
                }
            }
            con.Close();

            return marks;
        }

        public Boolean EditMark(string Faculty, string CourseCode, Marks mark)
        {
            SqlConnection con = new SqlConnectionGenerator().FromFaculty(Faculty);

            int res = -1;
            SqlCommand cmd = new SqlCommand("UPDATE " + CourseCode + " SET Mid=@mid, Attendence=@attendence, Assignment=@assignment, Final=@final WHERE StudentId=@id;", con);

            cmd.Parameters.AddWithValue("@mid", mark.Mid);
            cmd.Parameters.AddWithValue("@attendence", mark.Attendence);
            cmd.Parameters.AddWithValue("@assignment", mark.Assignment);
            cmd.Parameters.AddWithValue("@final", mark.Final);
            cmd.Parameters.AddWithValue("@id", mark.StudentId);
            try
            {
                con.Open();
                res = cmd.ExecuteNonQuery();
                if (res <= 0) return false;
            }
            catch (SqlException ex)
            {
                return false;
            }
            finally
            {
                con.Close();
            }

            return true;
        }

        public Boolean SubmitMark(string Faculty, string CourseCode, int StudentId, Boolean what)
        {
            SqlConnection con = new SqlConnectionGenerator().FromFaculty(Faculty);

            int res = -1;
            SqlCommand cmd = new SqlCommand("UPDATE " + CourseCode + " SET Submitted=@submitted WHERE StudentId=@id;", con);

            cmd.Parameters.AddWithValue("@submitted", what);
            cmd.Parameters.AddWithValue("@id", StudentId);
            try
            {
                con.Open();
                res = cmd.ExecuteNonQuery();
                if (res <= 0) return false;
            }
            catch (SqlException ex)
            {
                return false;
            }
            finally
            {
                con.Close();
            }

            return true;
        }

        public Boolean DeleteMark(string Faculty, string CourseCode, int StudentId)
        {
            SqlConnection con = new SqlConnectionGenerator().FromFaculty(Faculty);
            con.Open();

            int res = -1;
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + CourseCode + " WHERE StudentId=" + StudentId, con);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    reader.Close();
                    cmd = new SqlCommand("DELETE FROM " + CourseCode + " WHERE StudentId=@key AND Submitted=@submitted;", con);
                    cmd.Parameters.AddWithValue("@key", StudentId);
                    cmd.Parameters.AddWithValue("@submitted", false);
                    res = cmd.ExecuteNonQuery();
                    if (res <= 0) return false;
                }
                else
                {
                    return false;
                }
            }
            con.Close();

            return true;
        }

        public Boolean DeleteEntries(string[] Ids)
        {
            SqlConnection con = new SqlConnectionGenerator().FromFaculty(Ids[0]);
            con.Open();

            int res = -1;

            for (int i = 2; i < Ids.Length; i++)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM " + Ids[1] + " WHERE StudentId=@key AND Submitted=@submitted;", con);
                    cmd.Parameters.AddWithValue("@key", Ids[i]);
                    cmd.Parameters.AddWithValue("@submitted", false);
                    res = cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return false;
                }
            }
            con.Close();

            return true;
        }

        public string IsValid(List<Marks> marks)
        {
            StringBuilder sb = new StringBuilder();

            string str = "";
            foreach (Marks mark in marks)
            {
                if (mark.Mid > 15 || mark.Mid < -1) sb.Append("<p>" + Messasges.Invalid + " Mid mark(" + mark.Mid + ") out of range for ID: " + mark.StudentId + "</p>");
                if (mark.Attendence > 10 || mark.Attendence < -1) sb.Append("<p>" + Messasges.Invalid + " Attendence mark(" + mark.Attendence + ") out of range for ID: " + mark.StudentId + "</p>");
                if (mark.Assignment > 5 || mark.Assignment < -1) sb.Append("<p>" + Messasges.Invalid + " Assignment mark(" + mark.Assignment + ") out of range for ID: " + mark.StudentId + "</p>");
                if (mark.Final > 70 || mark.Final < -1) sb.Append("<p>" + Messasges.Invalid + " Final mark(" + mark.Final + ") out of range for ID: " + mark.StudentId + "</p>");
                
            }
            str = sb.ToString();
            if (str == "") return Messasges.Valid;

            return str;
        }

        public static string CalculateLetterGrade(float TotalMark)
        {
            if (TotalMark > 100 || TotalMark < -1) return null;

            string Grade = "F";

            int Percentage = (int) Math.Round(TotalMark);

            if (Percentage < 40) Grade = "F";
            else if (Percentage >= 40 && Percentage < 45) Grade = "D";
            else if (Percentage >= 45 && Percentage < 50) Grade = "C";
            else if (Percentage >= 50 && Percentage < 55) Grade = "C+";
            else if (Percentage >= 55 && Percentage < 60) Grade = "B-";
            else if (Percentage >= 60 && Percentage < 65) Grade = "B";
            else if (Percentage >= 65 && Percentage < 70) Grade = "B+";
            else if (Percentage >= 70 && Percentage < 75) Grade = "A-";
            else if (Percentage >= 75 && Percentage < 80) Grade = "A";
            else if (Percentage >= 80) Grade = "A+";
            else return null;

            return Grade;
        }

        public static float CalculateGP(float TotalMark)
        {
            if (TotalMark > 100 || TotalMark < -1) return 0;

            float GP = 0;

            int Percentage = (int) Math.Round(TotalMark);

            if (Percentage < 40) GP = 0;
            else if (Percentage >= 40 && Percentage < 45) GP = 2;
            else if (Percentage >= 45 && Percentage < 50) GP = 2.25f;
            else if (Percentage >= 50 && Percentage < 55) GP = 2.5f;
            else if (Percentage >= 55 && Percentage < 60) GP = 2.75f;
            else if (Percentage >= 60 && Percentage < 65) GP = 3;
            else if (Percentage >= 65 && Percentage < 70) GP = 3.25f;
            else if (Percentage >= 70 && Percentage < 75) GP = 3.5f;
            else if (Percentage >= 75 && Percentage < 80) GP = 3.75f;
            else if (Percentage >= 80) GP = 4;
            else return 0;

            return GP;
        }

        public static Boolean IsPassed(float TotalMark)
        {
            if (TotalMark < 40) return false;

            return true;
        }

        public static float CalculateCGPA(float CurrGPA, float CurrCCH, float PrevCGPA, float PrevCCH)
        {
            float CGPA = 0;

            float TotalCCH = CurrCCH + PrevCCH;
            if (TotalCCH == 0) return 0;

            CGPA = ((CurrGPA * CurrCCH) + (PrevCGPA * PrevCCH)) / TotalCCH;

            return CGPA;
        }

        public static float WrapToFloatPoint3(float point)
        {
            float WrappedPoint = 0;

            if (Convert.ToString(point).Length < 5) return point;
            else WrappedPoint = (float) Math.Round(point, 3);

            return WrappedPoint;
        }
    }
}