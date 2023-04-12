using System.Collections.Generic;

namespace CoreStoreMVC.Models.ViewModel
{
    public class AppointmentViewModel
    {
        public List<Appointment> Appointments { get; set; } = new();
        public PageInfo PaginationInfo { get; set; }
    }
}
