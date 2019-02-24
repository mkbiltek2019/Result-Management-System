using OfficeOpenXml;
using PRMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PRMS.Controllers
{
    public class SessionsController : Controller
    {

        private ProjectDB db = new ProjectDB();

        //
        // GET: /Faculty/
        public ActionResult Index()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("AddSession", "Sessions");
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


        public ActionResult AddSession()
        {
            //if (!HasSession()) return RedirectToAction("Index", "Home");

            Admin admin = HttpContext.Session[Variables.AdminSession] as Admin;
            if (admin != null)
            {
                return View(admin);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AddSession(int id, string session, HttpPostedFileBase file)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Admin admin = HttpContext.Session[Variables.AdminSession] as Admin;

            if (session != null && file != null)
            {
                string fileName = "", targetFileName = session.Trim().Replace("-", "_") + "_students";
                string content = "";

                if ((file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    fileName = Path.GetFileName(file.FileName);

                    if (fileName.ToLower().Contains(".xls") || fileName.ToLower().Contains(".xlsx"))
                    {
                        //read excel data
                        List<StudentInfo> stuInfo = new ExcelReader().readStuInfoFromExcel(file);
                        //insret student informations into StudentInfo table
                        if (stuInfo != null)
                        {
                            foreach (StudentInfo info in stuInfo)
                            {
                                content += info.StudentId + ",";

                                db.StudentInfo.Add(info);
                                db.SaveChanges();

                                ViewBag.Message = "Session Created Successfully!";
                            }

                            //insert new session into AllSession table
                            AllSession a = new AllSession();
                            a.Session = session;
                            a.Content = content;
                            db.AllSession.Add(a);
                            db.SaveChanges();

                            //add this activity into Activity table
                            try
                            {
                                //targetFileName += "_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_") + ".xlsx";
                                targetFileName += ".xlsx";
                                string path = Path.Combine(Server.MapPath("~/App_Data/Stu_Info/"), targetFileName);
                                file.SaveAs(path);
                                ViewBag.FileInfo = targetFileName + " uploaded successfully!";

                                AdminActivity activity = new AdminActivity();
                                activity.Name = admin.Username;
                                activity.UserId = Convert.ToString(admin.id);
                                activity.Time = DateTime.Now.ToString();
                                activity.Message = "New Session created: " + session;
                                activity.Content = content;
                                activity.FilePath = path;

                                db.AdminActivity.Add(activity);
                                db.SaveChanges();
                            }

                            catch (Exception ex)
                            {
                                ViewBag.Error = "ERROR:" + ex.Message.ToString();
                            }

                        }
                        else
                        {
                            ViewBag.Error = "ERROR:" + " Excel formet is Missmatche..!";
                        }
                    }
                    else
                    {
                        //format not supported
                        ViewBag.Message = "File formate not supported. Only .xls or .xlsx is required.";
                    }
                }
                else
                {
                    //empty file
                    ViewBag.Message = "Empty File.";
                }

            }
            else
            {
                ViewBag.Message = "Required field(s) missing.";
            }

            return View(admin);

        }

        public ActionResult ManageSession()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            List<AllSession> sessions = db.AllSession.ToList();

            return View(sessions);
        }

    }
}