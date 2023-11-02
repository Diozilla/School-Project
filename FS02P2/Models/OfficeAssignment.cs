using System.ComponentModel.DataAnnotations;

namespace FS02P2.Models
{
    public class OfficeAssignment
    {
        //pk and fk
        [Key]
        public int InstructorID { get; set; }
        [StringLength(50)]
        [Display(Name ="Office Location")]
        public string? Location { get; set; }
        public Instructor Instructor { get; set; }
    }
}
