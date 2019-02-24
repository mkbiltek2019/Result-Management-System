using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class CourseResult
    {
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public float CreditHours { get; set; }
        public float TotalMark { get; set; }
        public string LetterGrade { get; set; }
        public float GP { get; set; }
        public Boolean IsPassed { get; set; }

        public CourseResult CalculateCourseResult(Course Course, int StudentId)
        {
            if (Course == null || StudentId <= 0) return null;

            CourseResult CourseResult = new CourseResult();

            return CourseResult;
        }
    }
}