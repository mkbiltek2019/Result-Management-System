using PRMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PRMS.Controllers
{
    public class HistoryController : Controller
    {
        private ProjectDB db = new ProjectDB();
        //
        // GET: /History/
        public ActionResult Index()
        {
            Teacher teacher = HttpContext.Session[Variables.TeacherSession] as Teacher;
            if (teacher == null) return RedirectToAction("Index", "Home");

            string tId = teacher.TeacherID.ToString();
            List<TeacherActivity> activities = db.TeacherActivity.Where(b => b.UserId == tId).ToList();
            ViewBag.Activities = activities;

            return View(teacher);
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

        public ActionResult Details(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("DetailsHistory", "History", new { nid = id });
        }

        public ActionResult DetailsHistory(int? nid)
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
            ViewBag.marks = marks;
            ViewBag.id = nid;
            ViewBag.ActivityId = activity.id;
            ViewBag.Approved = activity.Approved;

            return View();
        }

        public ActionResult ChangeHistory(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("ChangeExcelHistory", "History", new { nid = id });
        }

        public ActionResult ChangeExcelHistory(int? nid)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (nid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Id = nid;
            return View();
        }

        [HttpPost]
        public ActionResult ChangeExcelHistory(int id, HttpPostedFileBase file)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            TeacherActivity activity = db.TeacherActivity.Find(id);
            string[] Ids = activity.Content.Split(',');
            //string content = Ids[0] + "," + Ids[1];

            if ((file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                //string fileContentType = Path.GetExtension(file.FileName).ToLower();
                string fileName = Path.GetFileName(file.FileName);
                if (fileName.ToLower().Contains(".xls") || fileName.ToLower().Contains(".xlsx"))
                {
                    //delete old marks
                    Boolean isDeleted = new Marks().DeleteEntries(Ids);

                    //insert new marks
                    List<Marks> marks = new ExcelReader().readMarksFromExcel(file);
                    if (marks != null)
                    {
                        string ValidityCheckRes = new Marks().IsValid(marks);
                        if (ValidityCheckRes.Equals(Messasges.Valid))
                        {
                            string res = new Marks().InserMarkList(marks, Ids[0], Ids[1]);

                            if (res.Contains(Messasges.InsertionFailed)) ViewBag.Error = Messasges.DataUpdateFailed;
                            else ViewBag.Message = Messasges.DataUpdatedSuccessfully;

                            try
                            {
                                file.SaveAs(activity.FilePath);

                                TeacherActivity newActivity = new TeacherActivity();
                                newActivity.Name = activity.Name;
                                newActivity.Designation = activity.Designation;
                                newActivity.CourseCode = activity.CourseCode;
                                newActivity.UserId = activity.UserId;
                                newActivity.Time = DateTime.Now.ToString();
                                newActivity.Message = activity.Message.Replace("uploaded", "changed");
                                newActivity.Content = res;
                                newActivity.FilePath = activity.FilePath;

                                db.TeacherActivity.Add(newActivity);
                                activity.FilePath = null;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ViewBag.FileInfo = "ERROR:" + ex.Message.ToString();
                            }

                            return RedirectToAction("Index", "History");
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
            else
            {
                ViewBag.Error = "No input data!";
            }

            ViewBag.Id = id;

            return View();
        }

        public ActionResult DeleteMarksFromExcel(int? id, int dest)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            TeacherActivity activity = db.TeacherActivity.Find(id);
            string[] Ids = activity.Content.Split(',');

            Boolean isDeleted = new Marks().DeleteEntries(Ids);

            if (isDeleted)
            {
                activity.FilePath = null;

                TeacherActivity newActivity = new TeacherActivity();
                newActivity.Name = activity.Name;
                newActivity.Designation = activity.Designation;
                newActivity.CourseCode = activity.CourseCode;
                newActivity.UserId = activity.UserId;
                newActivity.Time = DateTime.Now.ToString();
                newActivity.Message = activity.Message.Replace("uploaded", "deleted").Replace("changed", "deleted");

                db.TeacherActivity.Add(newActivity);
                db.SaveChanges();
            }

            if(dest == 0 ) return RedirectToAction("Index", "History");
            else if (dest == 1) return RedirectToAction("ApproveMarks", "Marks");
            else return RedirectToAction("Index", "Teacher");
        }
    }
}