using OfficeOpenXml;
using PRMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PRMS.Controllers
{
    public class ResultController : Controller
    {
        private ProjectDB db = new ProjectDB();
        //
        // GET: /Result/
        public ActionResult Index()
        {
            Admin admin = HttpContext.Session[Variables.AdminSession] as Admin;
            if (admin != null)
            {
                ViewBag.faculties = db.Faculty.ToList();
                ViewBag.all_session = db.AllSession.ToList();

                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Index(string Faculty, String Session, int? Semester, int? key)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (Faculty == null || Session == null || Semester == 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.faculties = db.Faculty.ToList();
            ViewBag.all_session = db.AllSession.ToList();
            ViewBag.Faculty = Faculty;
            ViewBag.Session = Session;
            ViewBag.Semester = Semester;
            List<Result> AllResults = new List<Result>();
            if (Convert.ToInt32(key) == 0)
            {
                AllResults = Result.GetResults(Session, Convert.ToInt32(Semester));
            }
            else
            {
                List<Course> Courses = new List<Course>();
                List<CurrentSemester> CS = new List<CurrentSemester>();
                Courses = GetEnrollment.GetEnrollCourses(Faculty, Session, Convert.ToInt32(Semester));
                CS = CurrentSemester.GetCurrentSemesterStudents(Faculty, Session, Convert.ToInt32(Semester));
                string msg = "";
                if (Courses != null)
                {
                    foreach (Course Course in Courses)
                    {
                        if (!CourseStatus.IsMarkSubmitted(Course, Session))
                        {
                            msg += "Mark is not submitted for Course: " + Course.Course_code + "\n";
                        }
                    }

                    if (msg == "")
                    {
                        List<StudentInfo> Students = StudentInfo.GetStudentInfo(CS);
                        if (Students != null)
                        {
                            Boolean IsCalculated = Result.CalculateResult(Students, Convert.ToInt32(Semester), Courses);
                            if (IsCalculated)
                            {
                                ViewBag.Message = "Results are calculated successfully!";
                                AllResults = Result.GetResults(Session, Convert.ToInt32(Semester));
                            }
                            else ViewBag.Error = "Results calculation failed!";
                        }
                    }
                    else ViewBag.Error = msg;
                }
                else
                {
                    ViewBag.Error = "No Courses and Marks are Found!";
                }
            }

            return View(AllResults);
        }

        public ActionResult ResultDetails(int? StudentId, int? Semester)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (StudentId == 0 || Semester == 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Result Result = Result.GetResult(Convert.ToInt32(StudentId), Convert.ToInt32(Semester));
            ViewBag.Result = Result;

            JavaScriptSerializer js = new JavaScriptSerializer();
            List<CourseResult> CourseResults = new List<CourseResult>();
            if(Result.CourseResults != null) CourseResults = js.Deserialize<List<CourseResult>>(Result.CourseResults);

            return View(CourseResults);
        }

        public ActionResult DownloadResult(string Faculty, string Session, int? Semester)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (Faculty == null || Session == null || Semester == 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.faculties = db.Faculty.ToList();
            ViewBag.all_session = db.AllSession.ToList();
            ViewBag.Faculty = Faculty;
            ViewBag.Session = Session;
            ViewBag.Semester = Semester;
            
            List<Result> AllResults = new List<Result>();
            AllResults = Result.GetResults(Session, Convert.ToInt32(Semester));

            if (AllResults == null) 
            {
                ViewBag.Error = "No Results Found!";
            }
            else
            {
                ViewBag.Message = "Result_" + Faculty + "_" + Session + "_" + Semester + " Downloaded Successfully!";

                ExcelPackage excel = ExcelWriter.CreateResultExcel(AllResults);
                string excelName = "Result_" + Faculty + "_" + Session + "_" + Semester;

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

            return View(AllResults);
        }

        protected Boolean HasSession()
        {
            Admin admin = HttpContext.Session[Variables.AdminSession] as Admin;
            if (admin == null)
            {
                return false;
            }

            return true;
        }
    }
}