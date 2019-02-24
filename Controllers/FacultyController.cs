using PRMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PRMS.Controllers
{
    public class FacultyController : Controller
    {

        private ProjectDB db = new ProjectDB(); 
        //
        // GET: /Faculty/
        public ActionResult Index()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("AddFaculty", "Faculty");
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


        public ActionResult AddFaculty()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public ActionResult AddFaculty([Bind(Include = "FacultyName,ShortForm")] Faculty faculty)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            faculty.ShortForm = faculty.ShortForm.ToUpper();

            if (db.Faculty.Where(a => a.ShortForm == faculty.ShortForm || a.FacultyName == faculty.FacultyName).FirstOrDefault() == null)
            {
                CreateDatabaseAndTable cdt = new CreateDatabaseAndTable(faculty.ShortForm);
                cdt.CreateDatabase(faculty.ShortForm);

                db.Faculty.Add(faculty);
                db.SaveChanges();
                ViewBag.Message = "Successfully Faculty Added..! ";
            }
            else
            {
                ViewBag.ErrorMessage = "Faculty Already Exists..!"; 
            }

            return View();
        }

        public ActionResult ManageFaculty()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return View(db.Faculty.ToList());

        }
        public ActionResult EditFaculty(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("EditFaculties", "Faculty", new { ind = id });
        }

        public ActionResult EditFaculties(int? ind)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (ind == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculty.Find(ind);
            if (faculty == null)
            {
                return HttpNotFound();
            }


            return View(faculty);
        }

        [HttpPost]
        public ActionResult EditFaculties([Bind(Include = "id,FacultyName,ShortForm")] Faculty faculty)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            faculty.ShortForm = faculty.ShortForm.ToUpper();

            Faculty oldFaculty = db.Faculty.Find(faculty.id);
            db.Entry(oldFaculty).State = EntityState.Detached;
            Boolean ck=false;
            if (!faculty.ShortForm.Equals(oldFaculty.ShortForm))
            {
                CreateDatabaseAndTable cdt = new CreateDatabaseAndTable(oldFaculty.ShortForm);
                  ck=  cdt.AlterDatabase(oldFaculty.ShortForm, faculty.ShortForm);
            }
           
            if (ck == false) return View(db.Faculty.Find(faculty.id));

            if (ModelState.IsValid)
            {
                db.Entry(faculty).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ManageFaculty", "Faculty");
        }


        public ActionResult DeleteFaculty(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Faculty faculty = db.Faculty.Find(id);
            db.Faculty.Remove(faculty);
            db.SaveChanges();
            return RedirectToAction("ManageFaculty", "Faculty");
        }
    
	}
}