using OfficeOpenXml;
using PRMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace PRMS.Controllers
{
    public class EnrollmentController : Controller
    {

        private ProjectDB db = new ProjectDB();
        //
        // GET: /Enrollment/
        public ActionResult Index()
        {   if (!HasSession()) return RedirectToAction("Index", "Home");
            ViewBag.faculties = db.Faculty.ToList();
            return View();
        }

        protected Boolean HasSession()
        {   Admin admin = HttpContext.Session[Variables.AdminSession] as Admin;
            if (admin == null)
            {
                return false;
            }
           return true;
        }

        [HttpPost]
        public ActionResult Index(String faculty, int semester)
        {   if (!HasSession()) return RedirectToAction("Index", "Home");
            String fileName = faculty + "_Enroll_" + semester + ".xlsx";
            Server.MapPath("~/App_Data/CSE/" + fileName);
            return RedirectToAction("UploadEnrollment", "Enrollment", new { faculty = faculty, semester = semester });
        }

       public ActionResult UploadEnrollment(String faculty, int semester)
        {   if (!HasSession()) return RedirectToAction("Index", "Home");
            String sem = SemesterName(semester);
            ViewBag.semester = semester;
            ViewBag.faculty = faculty;
            return View();
        }


        [HttpPost]
        public ActionResult UploadEnrollment(String faculty, int semester, HttpPostedFileBase postedFile)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            ViewBag.semester = semester;
            ViewBag.faculty = faculty;
            String fileName;
            if (faculty != null && postedFile != null)
            {

                if ((postedFile.ContentLength > 0) && !string.IsNullOrEmpty(postedFile.FileName))
                {
                    //string fileContentType = Path.GetExtension(file.FileName).ToLower();
                    fileName = Path.GetFileName(postedFile.FileName);

                    if (fileName.ToLower().Contains(".xls") || fileName.ToLower().Contains(".xlsx"))
                    {     byte[] fileBytes = new byte[postedFile.ContentLength];

                        var data = postedFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(postedFile.ContentLength));
                        using (var package = new ExcelPackage(postedFile.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            String property;
                            Boolean ck = false;

                            CRUD crd = new CRUD(faculty);
                            crd.TruncateCurrentSemester(semester-1);


                             for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                IDictionary<string, object> dict = new Dictionary<string, object>();
                                try
                                {
                                    for (int i = 1; i <= noOfCol; i++)
                                    {
                                        property = workSheet.Cells[1, i].Value.ToString();

                                        if (i == 1 || i == 4)
                                        {
                                            dict.Add(new KeyValuePair<string, object>(property, workSheet.Cells[rowIterator, i].Value));
                                        }
                                        else if (i == 2 || i == 3 || i == 5)
                                        {   
                                            dict.Add(new KeyValuePair<string, object>(property, Convert.ToInt32(workSheet.Cells[rowIterator, i].Value)));
                                            }
                                        else
                                        {
                                            if (workSheet.Cells[rowIterator, i].Value != null)
                                                dict.Add(new KeyValuePair<string, object>(property, Convert.ToBoolean(workSheet.Cells[rowIterator, i].Value)));
                                            else
                                                dict.Add(new KeyValuePair<string, object>(property, false));
                                        }
                                     }

                                    crd = new CRUD(faculty);
                                    ck = crd.InsertEnrollment(dict);
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.Message = Messasges.InvalidExcelData;
                                    return View();
                                }

                                if (ck == false)
                                {
                                    ViewBag.ck = ck;
                                    return View();
                                }
                            }
                           ViewBag.ck = ck;
                         }
                      }
                    else
                    {
                        //format not supported
                        ViewBag.Message = "File formate not supported. Only .xls or .xlsx is required.";
                    }
                }

            }
            else
            {
                ViewBag.Message = "Required field(s) missing.";
            }



            return View();
        }


        public ActionResult ManageEnrollment(string msg)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            ViewBag.msg = msg;
            ViewBag.faculties = db.Faculty.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult ManageEnrollment(string faculty, int semester, int id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            IDictionary<string, object> dict = new Dictionary<string, object>();

            CRUD crud = new CRUD(faculty);
            dict = crud.SelectEnrollment(semester, id);

            if (dict == null)
            {
                ViewBag.faculties = db.Faculty.ToList();
                ViewBag.message = "Student no found..!";
                return View("ManageEnrollment");
            }
            else
            {
                ViewBag.en = dict;
                ViewBag.faculty = faculty;
                ViewBag.semester = semester;

                return View("EditEnrollment");
            }
        }

        [HttpPost]
        public ActionResult EditEnrollment(String faculty, String name, int studentid, int regno, int semester, String session, String[] subjectlist)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            IDictionary<string, object> dict = new Dictionary<string, object>();

            CRUD crud = new CRUD(faculty);
            //     dict = 
            dict = crud.SelectEnrollment(semester, studentid);

            List<string> keys = new List<string>(dict.Keys);

            foreach (string item in keys)
            {
                if (dict[item].Equals(true))
                {
                    dict[item] = false;
                }

            }

            foreach (String s in subjectlist)
            {
                dict[s] = true;
            }
            crud.UpdateEnrollment(dict);

            dict = crud.SelectEnrollment(semester, studentid);

            ViewBag.en = dict;
            ViewBag.faculty = faculty;
            ViewBag.semester = semester;
            ViewBag.message = "Updated Successfully..!";
            return View();
        }

        public ActionResult Delete(int id, int semester, String faculty)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            IDictionary<string, object> dict = new Dictionary<string, object>();

            CRUD crud = new CRUD(faculty);
            dict = crud.SelectEnrollment(semester, id);
            Boolean ck = crud.DeleteEnrollment(id, semester);

            if (ck == true)
            {
                string message = "Student Enrollment Delete Successfully..!";
                return RedirectToAction("ManageEnrollment", "Enrollment", new { msg = message });

            }
            else
            {
                ViewBag.message = "Some Problem when Delete Student Enrollment..!";
                ViewBag.en = dict;
                ViewBag.faculty = faculty;
                ViewBag.semester = semester;
                return View("EditEnrollment");

            }


        }


        private static string SemesterName(int semester)
        {
            String sem = semester.ToString();

            switch (semester)
            {
                case 1:
                    sem += "st ";
                    break;
                case 2:
                    sem += "nd ";
                    break;
                case 3:
                    sem += "rd ";
                    break;
                case 4:
                    sem += "th ";
                    break;
                case 5:
                    sem += "th ";
                    break;
                case 6:
                    sem += "th ";
                    break;
                case 7:
                    sem += "th ";
                    break;
                case 8:
                    sem += "th ";
                    break;


            }
            return sem;
        }



    }
}