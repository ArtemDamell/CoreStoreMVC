using System.Collections.Generic;

namespace CoreStoreMVC.Models.ViewModel
{
    public class AppointmentDetailsViewModel
    {
        public Appointment Appointment { get; set; }
        public List<ApplicationUser> SalesPerson { get; set; }
        public List<Product> Products { get; set; }
    }
}
