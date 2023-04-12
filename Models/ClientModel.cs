using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CoreStoreMVC.Models
{
    public class ClientModel : IdentityUser
    {
        [Required]
        [MinLength(2), MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2), MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        [MinLength(2), MaxLength(50)]
        public string Adress { get; set; }

        [Required]
        public int Index { get; set; }

        [Required]
        [MinLength(2), MaxLength(50)]
        public string City { get; set; }
    }
}