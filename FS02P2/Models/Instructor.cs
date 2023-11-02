using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FS02P2.Models
{
    public class Instructor 
    {
        //base model class ---> table
        
            //properties --> columns
            public int InstructorID { get; set; }
             [Display(Name = "Last Name")]
             [Required(ErrorMessage = "Last name erquired")]
             [StringLength(30, ErrorMessage = "First name must be at max 30 characters")]
             [RegularExpression(@"^[A-Z]+[a-zA-Z]*$", ErrorMessage = "Must be normal characters only")]
             [Column("LastName")]

        public string? LastName { get; set; }

        [Required(ErrorMessage = "First name erquired")]
            [Display(Name = "First Name")]
            [StringLength(30, ErrorMessage = "Last name must be at max 30 characters")]
            [RegularExpression(@"^[A-Z]+[a-zA-Z]*$", ErrorMessage = "Must be normal characters only")]
            [Column("FirstName")]
            public string? FirstMidName { get; set; }
           

            [Display(Name ="Hire Date")]
            [DataType(DataType.Date)]
            [Required]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime? HireDate { get; set; }


            [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }
             public ICollection<CourseAssignment> CourseAssignments { get; set; }
             public OfficeAssignment OfficeAssignment { get; set; }
    }
        
}
