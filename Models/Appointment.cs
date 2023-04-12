using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreStoreMVC.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDay { get; set; }

        [NotMapped]
        public DateTime AppointmentTime { get; set; }

        [Required]
        [MinLength(2), MaxLength(30)]
        public string CustomerFirstName { get; set; }

        [Required]
        [MinLength(2), MaxLength(30)]
        public string CustomerLastName { get; set; }

        public string CustomerPhoneNumber { get; set; }

        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Required]
        [MinLength(2), MaxLength(50)]
        public string Adress { get; set; }

        [Required]
        public int Index { get; set; }

        [Required]
        [MinLength(2), MaxLength(50)]
        public string City { get; set; }

        public bool IsConfirmed { get; set; }

        [Display(Name = "Sales Person")]
        public string SalesPersonId { get; set; }

        [ForeignKey(nameof(SalesPersonId))]
        public virtual ApplicationUser SalesPerson { get; set; }
    }
}