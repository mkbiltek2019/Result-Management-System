using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Configuration;
using System.Data.SqlClient;
using PRMS.Models;
using System.Web.Configuration;
using System.Net;
using System.Data.Entity;
using System.Net.Mail;
using OfficeOpenXml.Style;


namespace PRMS.Controllers
{
    public class AdminController : Controller
    {

        private ProjectDB db = new ProjectDB();

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            Admin admin = HttpContext.Session[Variables.AdminSession] as Admin;
            if (admin != null)
            {
                ViewBag.username = admin.Username;
                ViewBag.email = admin.Email;
                ViewBag.faculties = db.Faculty.ToList();
                ViewBag.all_session = db.AllSession.ToList();
                ViewBag.Message = "Welcome to PSTU Result Management System!";
                return View();
            }

            return RedirectToAction("Index", "Home");
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

        public ActionResult LogOut()
        {
            if (HasSession())
            {
                HttpContext.Session.Abandon();
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddTeacher(string fullanme, string emailid, string mobile, string faculty, string department)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");
            
            ViewBag.faculties = db.Faculty.ToList();

            if (fullanme != null & emailid != null & mobile != null & faculty != null & department != null)
            {

                Teacher teacher = new Teacher(fullanme, emailid, mobile, faculty.ToUpper(), department.ToUpper(), new FacultyDepartmentInfo().createRandomPassword());
                if (db.Teacher.Where(a => a.Email == teacher.Email).FirstOrDefault() == null)
                {
                    db.Teacher.Add(teacher);
                    db.SaveChanges();
                    string Message = "<p>Now you are registered at PSTU Result Management System </p>";
                    Message += "<p>Username : " + teacher.Email + "</p>";
                    Message += "<p>Password : " + teacher.Password + "</p>";
                    Message += "<p>Please Change The password to Secure the System.</p>";

                    Utils.SendMail(teacher.Name, teacher.Email, Messasges.MailRegistrationInfo, Message);
                    ViewBag.Message = "Successfully Registered.. Please check the Mail.";
                }
                else
                {
                    ViewBag.ErrorMessage = teacher.Email + " Already Exsist in the Teacher List..!";
                }
            }

            return View();

        }
        public ActionResult TeachersList()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return View(db.Teacher.ToList());

        }
        public ActionResult Edit(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");
            
            return RedirectToAction("EditTeacher", "Admin", new { ind = id });

        }
        public ActionResult EditTeacher(int? ind)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (ind == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teacher.Find(ind);
            if (teacher == null)
            {
                return HttpNotFound();
            }

            FacultyDepartmentInfo facultyDeptInfo = new FacultyDepartmentInfo();
            ViewBag.faculties = db.Faculty.ToList();
            return View(teacher);

        }

        [HttpPost]
        public ActionResult EditTeacher([Bind(Include = "TeacherID,Name,Email,Mobile,Faculty,Department")] Teacher teacher)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            teacher.Faculty = teacher.Faculty.ToUpper();
            teacher.Department = teacher.Department.ToUpper();

            Teacher tcr = db.Teacher.Find(teacher.TeacherID);
            db.Entry(tcr).State = EntityState.Detached;

            teacher.Password = tcr.Password;

            if (ModelState.IsValid)
            {
                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("TeachersList", "Admin");
        }

        public ActionResult Delete(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Teacher teacher = db.Teacher.Find(id);
            db.Teacher.Remove(teacher);
            db.SaveChanges();
            return RedirectToAction("TeachersList", "Admin");
        }
        public ActionResult Detail(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("TeacherDetail", "Admin", new { ind = id });

        }
        public ActionResult TeacherDetail(int? ind)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (ind == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teacher.Find(ind);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }
        public ActionResult AddSemesters()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return View();
        }

        public ActionResult ChangePassword()
        {
            Admin admin = HttpContext.Session[Variables.AdminSession] as Admin;
            if (admin != null)
            {
                return View(admin);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ChangePassword(int id, string oldpassword, string newpassword, string retypenewpassword)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Admin admin = db.Admin.Find(id);

            if (newpassword.Equals(retypenewpassword))
            {
                //oldpassword = new EncryptionDectryption().Encryptdata(oldpassword);

                if (admin.Password.Equals(oldpassword))
                {
                    admin.Password = newpassword/*new EncryptionDectryption().Encryptdata(newpassword)*/;
                    db.Entry(admin).State = EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.Message = "Successfully password changed!";
                    //return RedirectToAction("index", "Teacher");
                    return View(admin);
                }
                else
                {
                    ViewBag.Message = "Invalid Old Password..!";

                    return View(admin);
                }
            }
            else
            {
                ViewBag.Message = "Miss Match new Password..!";
                return View(admin);
            }
        }

        public JsonResult GetDepartment(string faculty)
        {
            List<Department> dept = db.Department.Where(b => b.Faculty == faculty).ToList();
            //  db.Faculty.ToList();

            List<String> departments = new List<String>();

            foreach (Department dp in dept)
            {
                departments.Add(dp.ShortForm);
            }

            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHistory()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            List<TeacherActivity> activity = db.TeacherActivity.ToList();
            ViewBag.History = activity;

            return View();

        }
        [HttpPost]
        public ActionResult DownloadEnrollForm(string faculty, string session, int semester)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            List<Course> courseList = new List<Course>();

            courseList = db.Courses.Where(a => a.Semester == semester && a.UnderFaculty == faculty).ToList();


            if (courseList != null)
            {
                //export excel
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet0");
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 12;
                //Header of table  
                //  
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "Name";
                workSheet.Cells[1, 2].Value = "StudentId";
                workSheet.Cells[1, 3].Value = "RegNo";
                workSheet.Cells[1, 4].Value = "Session";
                workSheet.Cells[1, 5].Value = "Semester";


                int recordIndex = 6;

                foreach (Course crs in courseList)
                {
                    workSheet.Cells[1, recordIndex].Value = crs.Course_code;
                    recordIndex++;
                }

                workSheet.Column(1).AutoFit();
                workSheet.Column(2).AutoFit();
                workSheet.Column(3).AutoFit();
                workSheet.Column(4).AutoFit();
                workSheet.Column(5).AutoFit();
                string excelName = faculty + "_Semester" + semester + "_" + session;

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

            return RedirectToAction("Index", "Admin");
        }



        
    }

}