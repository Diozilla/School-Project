using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS02P2.Models
{
    //base class model --- table


    public class Course
    {
        //properties --- columns


        //if u want to inter the primary key of a table manually and prevent the database from generating it 

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name ="Number")]
        public int CourseID { get; set; }
        [StringLength(50,MinimumLength =3)]
        public string? Title {get; set; }

        [Range(0,5)]
        public int Credits { get; set; }


        //fk
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        
        //relations aka nav properties 
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<CourseAssignment>CourseAssignments { get; set; }
    }
}
