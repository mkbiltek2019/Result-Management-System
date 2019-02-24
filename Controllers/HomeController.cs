using PRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PRMS.Controllers
{
    public class HomeController : Controller
    {
        private ProjectDB db = new ProjectDB();


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string usertype, string username, string password)
        {
            try
            {
                username = username.Replace(" ", String.Empty);
                password = password.Replace(" ", String.Empty);
            }
            catch (NullReferenceException e)
            {

            }

            if (username != null && password != null)
            {
                if (usertype.Equals("admin"))
                {
                    Admin admin = db.Admin.Where(a => a.Username == username && a.Password == password).FirstOrDefault();

                    if (admin != null)
                    {
                        HttpContext.Session[Variables.AdminSession] = admin;
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        ViewBag.Message = "Invalid Username or Password!";
                    }


                }
                else if (usertype.Equals("teacher"))
                {
                    Teacher teacher = db.Teacher.Where(a => a.Email == username && a.Password == password).FirstOrDefault();

                    if (teacher != null)
                    {
                        HttpContext.Session[Variables.TeacherSession] = teacher;

                        Boolean IsChairman = false;
                        Department department= db.Department.Where(d => d.ChairmanId == teacher.TeacherID).FirstOrDefault();
                        if (department != null) IsChairman = true;
                        HttpContext.Session[Variables.IsChairmanSession] = IsChairman;

                        return RedirectToAction("Index", "Teacher");
                    }
                    else
                    {
                        ViewBag.Message = "Invalid Username or Password!";
                    }
                }
            }

            return View();
        }

        public JsonResult RecoverPassword(string usertype, string email)
        {
            string result = "";
            if ((usertype == null || email == null) || (usertype.Length == 0 || email.Length == 0)) return Json(Messasges.RequiredFiledsMissing, JsonRequestBehavior.AllowGet);

            if (usertype.Equals("admin"))
            {
                Admin admin = db.Admin.Where(a => a.Username == email).FirstOrDefault();

                if (admin != null)
                {
                    string Message = "<p>Your Recovery Information Is Given Below</p>";
                    Message += "<p>Password : " + admin.Password + "</p>";
                    Utils.SendMail("Admin", admin.Email, Messasges.MailPasswordRecovery, Message);
                    result = "Recovery Information Is Sent To Your Email.";
                }
                else
                {
                    result = Messasges.InvalidUsername;
                }


            }
            else if (usertype.Equals("teacher"))
            {
                Teacher teacher = db.Teacher.Where(a => a.Email == email).FirstOrDefault();

                if (teacher != null)
                {
                    string Message = "<p>Your Recovery Information Is Given Below</p>";
                    Message += "<p>Password : " + teacher.Password + "</p>";
                    Utils.SendMail(teacher.Name, teacher.Email, Messasges.MailPasswordRecovery, Message);
                    result = "Recovery Information Is Sent To Your Email.";
                }
                else
                {
                    result = Messasges.InvalidEmail;
                }
            }
            else result = Messasges.InvalidRequest;

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}