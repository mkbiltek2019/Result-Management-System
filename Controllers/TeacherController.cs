using PRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using System.IO;
using OfficeOpenXml;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using OfficeOpenXml.Style;

namespace PRMS.Controllers
{
    public class TeacherController : Controller
    {
        //
        // GET: /Teacher/

        private ProjectDB db = new ProjectDB();

        public ActionResult Index()
        {
            Teacher teacher = HttpContext.Session[Variables.TeacherSession] as Teacher;
            if (teacher != null)
            {
                ViewBag.faculties = db.Faculty.ToList();
                ViewBag.all_session = db.AllSession.ToList();
                ViewBag.Message = "Welcome " + teacher.Name + "!";
                return View();
            }
            //  ViewBag.Email = teacher.Email;  

            return RedirectToAction("Index", "Home");
        }

        protected Boolean HasSession()
        {
            Teacher teacher = HttpContext.Session[Variables.TeacherSession] as Teacher;
            if (teacher == null)
            {
                return false;
            }

            return true;
        }

        public ActionResult LogOut()
        {
            if (HasSession())
            {
                HttpContext.Session.Abandon();
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePassword(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Teacher teacher = db.Teacher.Find(id);
            return View(teacher);
        }

        [HttpPost]
        public ActionResult ChangePassword(int? TeacherID, String oldpassword, String newpassword, String retypenewpassword)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Teacher teacher = db.Teacher.Find(TeacherID);

            if (newpassword.Equals(retypenewpassword))
            {


                if (teacher.Password.Equals(oldpassword))
                {
                    teacher.Password = newpassword;

                    db.Entry(teacher).State = EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.Message = "Successfully password changed";
                    //return RedirectToAction("index", "Teacher");
                    return View(teacher);
                }
                else
                {
                    ViewBag.Message = "Invalid Old Password..!";

                    return View(teacher);
                }
            }
            else
            {
                ViewBag.Message = "Miss Match new Password..!";
                return View(teacher);
            }
        }

        public JsonResult GetCourses(int semester, string faculty)
        {
            List<Course> courses = db.Courses.Where(b => b.Semester == semester && b.UnderFaculty == faculty).ToList();
            List<String> course_codes = new List<String>();
            foreach (Course course in courses)
            {
                course_codes.Add(course.Course_code);
            }
            return Json(course_codes, JsonRequestBehavior.AllowGet);
        }
        

        [HttpPost]
        public ActionResult DownloadEnroll(string faculty, string session, int semester, string course_code)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            List<EnrolledStudent> studentList = new List<EnrolledStudent>();

            GetEnrollment getEnrollment = new GetEnrollment(faculty);
            studentList = getEnrollment.GetStudentEnrollment(course_code, semester);

            if (studentList != null)
            {
                //export excel
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 12;
                //Header of table  
                //  
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "StudentId";
                workSheet.Cells[1, 2].Value = "RegNo";
                workSheet.Cells[1, 3].Value = "Name";
                workSheet.Cells[1, 4].Value = "Mid";
                workSheet.Cells[1, 5].Value = "Attendence";
                workSheet.Cells[1, 6].Value = "Assignment";
                workSheet.Cells[1, 7].Value = "Final";

                int recordIndex = 2;

                foreach (EnrolledStudent s in studentList)
                {
                    workSheet.Cells[recordIndex, 1].Value = s.StudentId;
                    workSheet.Cells[recordIndex, 2].Value = s.RegNo;
                    workSheet.Cells[recordIndex, 3].Value = s.Name;

                  
                    if (s.Mid == -1) workSheet.Cells[recordIndex, 4].Value = "Absent";
                    else if (s.Mid != 1000) workSheet.Cells[recordIndex, 4].Value = s.Mid;
                    if (s.Attendence == -1) workSheet.Cells[recordIndex, 5].Value = "Absent";
                    else if (s.Attendence != 1000) workSheet.Cells[recordIndex, 5].Value = s.Attendence;
                    if (s.Assignment == -1) workSheet.Cells[recordIndex, 6].Value = "Absent";
                    else if (s.Assignment != 1000) workSheet.Cells[recordIndex, 6].Value = s.Assignment;
                    

                    recordIndex++;
                }
                workSheet.Column(1).AutoFit();
                workSheet.Column(2).AutoFit();
                workSheet.Column(3).AutoFit();
                workSheet.Column(4).AutoFit();
                workSheet.Column(5).AutoFit();
                workSheet.Column(6).AutoFit();
                string excelName = faculty + "_" + course_code + "_" + session;

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }
            return View();
        }
    }
}