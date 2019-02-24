using PRMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PRMS.Controllers
{
    public class CourseController : Controller
    {
        private ProjectDB db = new ProjectDB();
        //
        // GET: /Faculty/
        public ActionResult Index()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("AddCourse", "Course");
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


        public ActionResult AddCourse()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            ViewBag.faculties = db.Faculty.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddCourse([Bind(Include = "Course_code,Course_title,Credit_hour,Semester,UnderFaculty,UnderDepartment,CourseTeacherID")] Course course)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            course.Course_code = course.Course_code.Trim().Replace(" ", "_").Replace("-", "_").ToUpper();
            Course crs = db.Courses.Where(a => a.Course_code == course.Course_code).FirstOrDefault();

            string message = course.Course_code + " Course Already Exsist.";
            if (crs == null)
            {
                CreateDatabaseAndTable cdt = new CreateDatabaseAndTable(course.UnderFaculty);
                message = cdt.AddCourse(course.Course_code, course.Semester);
                Teacher tcr = db.Teacher.Where(dp => dp.TeacherID == course.CourseTeacherID).FirstOrDefault();
                if (tcr != null)
                {
                    course.CourseTeacherName = tcr.Name;
                    course.CourseTeacherDepartment = tcr.Department;
                    course.CourseTeacherFaculty = tcr.Faculty;
                }
                db.Courses.Add(course);
                db.SaveChanges();
            }

            ViewBag.Message = message + " Under "
                   + course.UnderFaculty + " faculty, under " + course.UnderDepartment + " department.";

            //ViewBag.Message = course.Course_code + " added failed or already exists.";
            ViewBag.faculties = db.Faculty.ToList();

            return View();
        }

        public ActionResult ManageCourse()
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            ViewBag.faculties = db.Faculty.ToList();
            return View(db.Courses.ToList());

        }
        public ActionResult EditMe(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            return RedirectToAction("EditCourse", "Course", new { ind = id });
        }
        public ActionResult EditCourse(int? ind)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            if (ind == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(ind);
            if (course == null)
            {
                return HttpNotFound();
            }

            ViewBag.faculties = db.Faculty.ToList();
            return View(course);
        }

        [HttpPost]
        public ActionResult EditCourse([Bind(Include = "id,Course_code,Course_title,Credit_hour,Semester,UnderFaculty,CourseTeacherID")] Course course)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Course crs = db.Courses.Find(course.id);
            db.Entry(crs).State = EntityState.Detached;
            course.Course_code = course.Course_code.Trim().Replace(" ", "_").Replace("-", "_").ToUpper();

            if (!crs.Course_code.Equals(course.Course_code))
            {
                CreateDatabaseAndTable cdt = new CreateDatabaseAndTable(crs.UnderFaculty);
                cdt.AlterCourseTable(crs.Course_code, course.Course_code);
                string table = (course.Semester % 2 == 1) ? "JanEnrollment" : "JulEnrollment";
                cdt.RenameTableColumn(table, crs.Course_code, course.Course_code);

            }
            if (ModelState.IsValid)
            {
                Teacher tcr = db.Teacher.Where(dp => dp.TeacherID == course.CourseTeacherID).FirstOrDefault();
                if (tcr != null)
                {
                    course.CourseTeacherName = tcr.Name;
                    course.CourseTeacherDepartment = tcr.Department;
                    course.CourseTeacherFaculty = tcr.Faculty;
                }
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = course.Course_code + " Updated!";
            }
            else ViewBag.Error = course.Course_code + " Not Updated!";

            return RedirectToAction("ManageCourse", "Course");
        }


        public ActionResult DeleteCourse(int? id)
        {
            if (!HasSession()) return RedirectToAction("Index", "Home");

            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            ViewBag.Message = course.Course_code + " Deleted!";
            return RedirectToAction("ManageCourse", "Course");
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

        public JsonResult GetTeacherList(string faculty, string department)
        {
            List<Teacher> teachers = db.Teacher.Where(b => b.Faculty == faculty && b.Department == department).ToList();
            //  db.Faculty.ToList();

            List<String> teachernames = new List<String>();

            foreach (Teacher dp in teachers)
            {
                teachernames.Add(dp.Name+"*"+dp.TeacherID);
            }


            return Json(teachernames, JsonRequestBehavior.AllowGet);
        }

    }
}