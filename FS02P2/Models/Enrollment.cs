using System.ComponentModel.DataAnnotations;

namespace FS02P2.Models
{
    //enum for grades cuz we cant get out of those grades single type  limited 
    public enum Grade {
        A, B, C, D,F,NA
    }
    //base class model --- table in this case  shared table between student and course
    public class Enrollment
    {
        //properteis ---- columns 
        public int EnrollmentID { get; set; }

        public int StudentID { get; set; }
        public int CourseID { get; set; }

        [DisplayFormat(NullDisplayText ="No Grade")]
        public Grade? Grade { get; set; }

        //relations aka nav properties
        public Course? Course { get; set; }

        public Student? Student { get; set; }

    }
}
