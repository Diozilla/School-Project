using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS02P2.Models
{
    //base model class ---> table
    public class Student 
    {
        //properties --> columns
        public int StudentID { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name erquired")]
        [StringLength(30, ErrorMessage = "First name must be at max 30 characters")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z]*$", ErrorMessage = "Must be normal characters only")]
        [Column("LastName")]

        public string? LastName { get; set; }

        [Required(ErrorMessage ="First name erquired")]
        [Display(Name = "First Name")]
        [StringLength(30,ErrorMessage ="Last name must be at max 30 characters")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z]*$",ErrorMessage ="Must be normal characters only")]
        [Column("FirstName")]
        public string? FirstMidName { get; set;}
        
        [Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EnrollmentDate { get; set; }
       
        
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }
        //relations 1 to many with enrollments aka navigation properties

        public ICollection<Enrollment>? Enrollments { get; set; }

    }
}
