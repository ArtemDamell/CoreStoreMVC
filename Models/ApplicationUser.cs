using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CoreStoreMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Sales Person")]
        [MaxLength(30)]
        public string Name { get; set; }

        [NotMapped]
        public bool IsSuperAdmin { get; set; }
    }
}