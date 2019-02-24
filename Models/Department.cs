using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class Department
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { set; get; }
        public String DepartmentName { set; get; }
        public String ShortForm { set; get; }
        public String Faculty { set; get; }
        public int ChairmanId { set; get; }
        public String ChairmanName { get; set; }
        public String ChairmanEmail { get; set; }
    }
}