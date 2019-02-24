using PRMS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PRMS.Controllers
{
    public class MarksController : Controller
    {
        private ProjectDB db = new ProjectDB();

        //
        // GET: /Marks/
        public ActionResult Index()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("UploadMark", "Marks");
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

        public ActionResult UploadMark(int? activityId)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");
            Teacher teacher = HttpContext.Session[Variables.TeacherSession] as Teacher;

            ViewBag.courses = db.Courses.Where(dp => dp.CourseTeacherID == teacher.TeacherID).ToList();
            ViewBag.faculties = db.Faculty.ToList();
            ViewBag.all_session = db.AllSession.ToList();

            if (activityId != null)
            {
                TeacherActivity activity = db.TeacherActivity.Where(p => p.id == activityId).FirstOrDefault();

                string[] arr = activity.Content.Split(',');

                List<Marks> marks = new List<Marks>();

                if (arr != null)
                {
                    ViewBag.Faculty = arr[0];
                    ViewBag.Course = arr[1];

                    for (int i = 2; i < arr.Length; i++)
                    {
                        marks.Add(new Marks().GetMark(arr[0], arr[1], Convert.ToInt32(arr[i])));
                    }
                }
                ViewBag.Marks = marks;
                ViewBag.Course = arr[1];
                ViewBag.Faculty = arr[0];
                ViewBag.ActivityId = activityId;
            }

            return View();
        }

        [HttpPost]
        public ActionResult UploadMark(string faculty, string session, string course_code, HttpPostedFileBase file, string msg)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");
            Teacher teacher = HttpContext.Session[Variables.TeacherSession] as Teacher;

            if (faculty != null && session != null && course_code != null && file != null)
            {
                course_code = course_code.Trim().Replace(" ", "_").ToUpper();
                string fileName = "", targetFileName = course_code + "_" + session.Replace("-", "_");

                if ((file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    //string fileContentType = Path.GetExtension(file.FileName).ToLower();
                    fileName = Path.GetFileName(file.FileName);

                    if (fileName.ToLower().Contains(".xls") || fileName.ToLower().Contains(".xlsx"))
                    {
                        List<Marks> marks = new ExcelReader().readMarksFromExcel(file);

                        if (marks != null)
                        {
                            string ValidityCheckRes = new Marks().IsValid(marks);
                            if (ValidityCheckRes.Equals(Messasges.Valid))
                            {
                                string InsertionRes = new Marks().InserMarkList(marks, faculty, course_code);
                                ViewBag.Marks = marks;
                                ViewBag.Course = course_code;
                                ViewBag.Faculty = faculty;

                                if (!InsertionRes.Contains(Messasges.InsertionFailed))
                                {
                                    ViewBag.Message = Messasges.DataUploadedSuccessfully;
                                    CourseStatus.AddCourseStatus(course_code, session, false);

                                    try
                                    {
                                        //targetFileName += "_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + ".xlsx";
                                        targetFileName += ".xlsx";
                                        string path = Path.Combine(Server.MapPath("~/App_Data/Marks/"), targetFileName);
                                        file.SaveAs(path);
                                        ViewBag.FileInfo = targetFileName + " uploaded successfully!";

                                        TeacherActivity activity = new TeacherActivity();
                                        activity.Name = teacher.Name;
                                        activity.Designation = teacher.Department;
                                        activity.CourseCode = course_code;
                                        activity.Session = session;
                                        activity.UserId = teacher.TeacherID.ToString();
                                        activity.Time = DateTime.Now.ToString();
                                        msg = "Marks uploaded for: " + course_code + ", " + session + ".\n" + msg;
                                        activity.Message = msg;
                                        activity.Content = InsertionRes;
                                        activity.FilePath = path;

                                        db.TeacherActivity.Add(activity);
                                        db.SaveChanges();

                                        TeacherActivity act = db.TeacherActivity.Where(p => p.Time == activity.Time).FirstOrDefault();
                                        ViewBag.ActivityId = act.id;

                                    }
                                    catch (Exception ex)
                                    {
                                        ViewBag.Error = "ERROR:" + ex.Message.ToString();
                                    }
                                }
                                else
                                {
                                    ViewBag.Error = Messasges.DataUploadFailed;
                                }
                            }
                            else
                            {
                                ViewBag.Error = ValidityCheckRes;
                            }

                        }
                        else
                        {
                            ViewBag.Error = Messasges.InvalidExcelData;
                        }
                    }
                    else
                    {
                        //format not supported
                        ViewBag.Error = Messasges.InvalidExcelFileFormate;
                    }
                }

            }
            else
            {
                ViewBag.Error = Messasges.RequiredFiledsMissing;
            }

            ViewBag.courses = db.Courses.Where(dp => dp.CourseTeacherID == teacher.TeacherID).ToList();
            ViewBag.faculties = db.Faculty.ToList();
            ViewBag.all_session = db.AllSession.ToList();

            return View();
        }

        public ActionResult ApproveMarks()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Boolean IsChairman = Convert.ToBoolean(HttpContext.Session[Variables.IsChairmanSession]);
            if (!IsChairman) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Teacher teacher = HttpContext.Session[Variables.TeacherSession] as Teacher;
            ViewBag.Department = teacher.Department;

            List<TeacherActivity> activities = db.TeacherActivity.Where(p => p.Designation == teacher.Department).ToList();
            ViewBag.Activities = activities;

            return View();
        }

        public ActionResult Approve(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Boolean IsChairman = Convert.ToBoolean(HttpContext.Session[Variables.IsChairmanSession]);
            if (!IsChairman || id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TeacherActivity activity = db.TeacherActivity.Where(p => p.id == id).FirstOrDefault();
            string[] arr = activity.Content.Split(',');
            if (arr != null)
            {
                ViewBag.Faculty = arr[0];
                ViewBag.Course = arr[1];

                for (int i = 2; i < arr.Length; i++)
                {
                    new Marks().SubmitMark(arr[0], arr[1], Convert.ToInt32(arr[i]), true);
                }
            }
            CourseStatus.UpdateCourseStatus(activity.CourseCode, activity.Session, true);

            activity.Approved = true;
            db.SaveChanges();

            return RedirectToAction("ApproveMarks", "Marks");
        }

        public ActionResult Details(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("DetailsMarks", "Marks", new { nid = id });
        }

        public ActionResult DetailsMarks(int? nid)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (nid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TeacherActivity activity = db.TeacherActivity.Where(p => p.id == nid).FirstOrDefault();

            string[] arr = activity.Content.Split(',');

            List<Marks> marks = new List<Marks>();

            if (arr != null)
            {
                ViewBag.Faculty = arr[0];
                ViewBag.Course = arr[1];

                for (int i = 2; i < arr.Length; i++)
                {
                    marks.Add(new Marks().GetMark(arr[0], arr[1], Convert.ToInt32(arr[i])));
                }
            }
            ViewBag.Course = activity.CourseCode;
            ViewBag.marks = marks;
            ViewBag.id = nid;
            ViewBag.ActivityId = activity.id;
            ViewBag.Approved = activity.Approved;

            return View();
        }

        public ActionResult ManageMarks()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Boolean IsChairman = Convert.ToBoolean(HttpContext.Session[Variables.IsChairmanSession]);
            if (!IsChairman) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Teacher teacher = HttpContext.Session[Variables.TeacherSession] as Teacher;
            ViewBag.Department = teacher.Department;

            List<TeacherActivity> activities = db.TeacherActivity.Where(p => p.Designation == teacher.Department).ToList();
            ViewBag.Activities = activities;

            return View();
        }

        [HttpPost]
        public ActionResult ManageMarks(string Faculty, string session, string semester, string course_code)
        {
            Boolean IsChairman = Convert.ToBoolean(HttpContext.Session[Variables.IsChairmanSession]);

            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (!IsChairman) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (course_code == null || session == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            List<Marks> marks = new Marks().GetMarkList(Faculty, course_code, session);

            if (marks != null)
            {
                ViewBag.Marks = marks;
                ViewBag.Message = "Marks showing for " + course_code + ", Session: " + session;
            }
            else
            {
                ViewBag.Error = "No data found for " + course_code + ", Session: " + session;
            }

            ViewBag.faculties = db.Faculty.ToList();
            ViewBag.all_session = db.AllSession.ToList();
            ViewBag.Faculty = Faculty;
            ViewBag.Course = course_code;

            return View();
        }

        public ActionResult EditMark(int? stuId, string faculty, string course_code, int dest, int activityId)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("ChangeMark", "Marks", new { nid = stuId, nfaculty = faculty, ncourse_code = course_code, dest = dest, activityId = activityId });
        }

        public ActionResult ChangeMark(int nid, string nfaculty, string ncourse_code, int dest, int activityId)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (nid == null || nfaculty == null || ncourse_code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Faculty = nfaculty;
            ViewBag.Course_code = ncourse_code;
            ViewBag.Dest = dest;
            ViewBag.ActivityId = activityId;

            Marks mark = new Marks().GetMark(nfaculty, ncourse_code, nid);
            if (mark == null)
            {
                ViewBag.Error = Messasges.RecordNotFound;
                return HttpNotFound();
            }
            else ViewBag.Mark = mark;

            return View();
        }

        [HttpPost]
        public ActionResult ChangeMark(string faculty, string course_code, int dest, int activityId, [Bind(Include = "StudentId,RegNo,Mid,Attendence,Assignment,Final")] Marks mark)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (course_code == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.Mark = mark;

            Boolean isEdited = new Marks().EditMark(faculty, course_code, mark);
            if (isEdited) ViewBag.Message = Messasges.DataUploadedSuccessfully;
            else ViewBag.Error = Messasges.DataUpdateFailed;

            if (dest == 0) return RedirectToAction("UploadMark", "Marks", new { activityId = activityId });
            else return RedirectToAction("Details", "History", new { id = activityId });
            //return View();
        }

        public ActionResult DeleteMark(int stuId, string faculty, string course_code, int dest, int activityId)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            ViewBag.faculties = db.Faculty.ToList();
            ViewBag.all_session = db.AllSession.ToList();

            Boolean isDeleted = new Marks().DeleteMark(faculty, course_code, stuId);

            if (isDeleted) ViewBag.Message = Messasges.DataDeletedSuccessfully;
            else ViewBag.Message = "Record not found or " + Messasges.DeletionFailed;

            if (dest == 0) return RedirectToAction("UploadMark", "Marks", new { activityId = activityId });
            else if(dest == 1) return RedirectToAction("Details", "History", new { id = activityId });
            else if(dest == 2) return RedirectToAction("Details", "Marks", new { id = activityId });
            else return RedirectToAction("Index", "Teacher");
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
    }
}