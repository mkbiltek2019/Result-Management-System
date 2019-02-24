using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;  
namespace PRMS.Models
{
    public class Teacher
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TeacherID { set; get; }
        public String Name { set; get; }
        public String Email { set; get; }
        public String Mobile { set; get; }
        public String Faculty { set; get; }
        public String Department { set; get; }
        public String Password { set; get; }
       
        

        public Teacher(string fullanme, string emailid, string mobile, string faculty, string department,string password)
        {
            Name = fullanme;
            Email = emailid;
            Mobile = mobile;
            Faculty = faculty;
            Department = department;
            Password = password;
         

        }
        public Teacher(int teacherid, string fullanme, string emailid, string mobile, string faculty, string department)
        {
            TeacherID = teacherid;
            Name = fullanme;
            Email = emailid;
            Mobile = mobile;
            Faculty = faculty;
            Department = department;
        }
        public Teacher()
        {
            // TODO: Complete member initialization
        }
    }
}