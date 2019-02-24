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
    public class DepartmentController : Controller
    {

        private ProjectDB db = new ProjectDB();
        //
        // GET: /Faculty/
        public ActionResult Index()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("AddDepartment", "Department");
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

        public ActionResult AddDepartment()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            ViewBag.faculties = db.Faculty.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddDepartment([Bind(Include = "DepartmentName,ShortForm,Faculty,ChairmanName")] Department department)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            department.ShortForm = department.ShortForm.ToUpper();
            department.Faculty = department.Faculty.ToUpper();

            Teacher tcr = db.Teacher.Where(a => a.Name == department.ChairmanName && a.Faculty == department.Faculty).FirstOrDefault();
            if (tcr != null)
            {
                department.ChairmanId = tcr.TeacherID;
                department.ChairmanEmail = tcr.Email;
            }
            else
            {
                department.ChairmanName = null;
            }

            if (db.Department.Where(a => a.ShortForm == department.ShortForm || a.DepartmentName == department.DepartmentName).FirstOrDefault() == null)
            {
                db.Department.Add(department);
                db.SaveChanges();
                ViewBag.Message = "Successfully Department Added..! ";
            }
            else
            {
                ViewBag.ErrorMessage = "Department Already Exists..!";
            }

            ViewBag.faculties = db.Faculty.ToList();
            return View();
        }

        public ActionResult ManageDepartment()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            ViewBag.faculties = db.Faculty.ToList();
            return View(db.Department.ToList());

        }
        public ActionResult EditDepartment(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("EditDepartments", "Department", new { ind = id });
        }

        public ActionResult EditDepartments(int? ind)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (ind == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Department.Find(ind);

            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.teachers = db.Teacher.Where(a => a.Department == department.ShortForm).ToList();
            ViewBag.faculties = db.Faculty.ToList();
            return View(department);
        }

        [HttpPost]
        public ActionResult EditDepartments([Bind(Include = "id,DepartmentName,ShortForm,Faculty,ChairmanId")] Department department)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            department.ShortForm = department.ShortForm.ToUpper();
            department.Faculty = department.Faculty.ToUpper();

            Teacher tcr = db.Teacher.Where(a => a.TeacherID == department.ChairmanId && a.Faculty == department.Faculty).FirstOrDefault();
            if (tcr != null)
            {
                department.ChairmanName = tcr.Name;
                department.ChairmanId = tcr.TeacherID;
                department.ChairmanEmail = tcr.Email;
            }
            else
            {
                department.ChairmanName = null;
                department.ChairmanId = 0;
                department.ChairmanEmail = null;
            }

            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ManageDepartment", "Department");
        }


        public ActionResult DeleteDepartment(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Department department = db.Department.Find(id);
            db.Department.Remove(department);
            db.SaveChanges();
            return RedirectToAction("ManageDepartment", "Department");
        }

        public JsonResult GetTeachers(string faculty)
        {
            List<Teacher> teacher = db.Teacher.Where(b => b.Faculty == faculty).ToList();
            List<String> departments = new List<String>();
            departments.Add("Not Registered");
            foreach (Teacher tcr in teacher)
            {
                departments.Add(tcr.Name);
            }
            return Json(departments, JsonRequestBehavior.AllowGet);
        }


    }
}