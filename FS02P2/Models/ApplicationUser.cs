using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FS02P2.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(30)]
        public string? LastName { get; set; }
        [StringLength(30)]

        public string? FirstName { get; set; }

        public string FullName { get{ return FirstName + " " + LastName; } }


    }
}
