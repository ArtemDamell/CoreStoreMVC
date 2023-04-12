using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using CoreStoreMVC.Models.ViewModel;
using CoreStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreStoreMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    [Area(nameof(Admin))]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        int pageSize = 3;

        public AppointmentViewModel appointmentVM { get; set; } = new();

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a list of appointments from the database based on the search key and page number.
        /// </summary>
        /// <param name="searchKey">The search key used to filter the appointments.</param>
        /// <param name="productPage">The page number of the appointments.</param>
        /// <returns>A view containing the list of appointments.</returns>
        [HttpGet]
        public async Task<IActionResult> Index(string searchKey, int productPage = 1)
        {
            var currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            StringBuilder param = new();
            param.Append("/Admin/Appointments?productPage=:");
            param.Append("&searchKey=");
            if (searchKey is not null)
                param.Append(searchKey);

            if (searchKey is null)
                appointmentVM.Appointments = await _db.Appointments.Include(x => x.SalesPerson).ToListAsync();
            else
            {
                appointmentVM.Appointments = await _db.Appointments.Include(x => x.SalesPerson).Where(x => x.CustomerFirstName.ToLower().Contains(searchKey.ToLower()) ||
                                                                                          x.CustomerLastName.ToLower().Contains(searchKey.ToLower()) ||
                                                                                          x.CustomerEmail.Contains(searchKey) ||
                                                                                          x.CustomerPhoneNumber.Contains(searchKey)).ToListAsync();
            }

            if (User.IsInRole(SD.AdminEndUser))
                appointmentVM.Appointments = appointmentVM.Appointments.Where(x => x.SalesPersonId == claim.Value).ToList();

            var count = appointmentVM.Appointments.Count;

            appointmentVM.Appointments = appointmentVM.Appointments.OrderBy(p => p.AppointmentDay)
                                                                   .Skip((productPage - 1) * pageSize)
                                                                   .Take(pageSize).ToList();

            appointmentVM.PaginationInfo = new()
            {
                CurrentPage = productPage,
                ItemsPerPage = pageSize,
                TotalItems = count,
                UrlParam = param.ToString()
            };

            return View(appointmentVM);
        }

        /// <summary>
        /// Retrieves the details of an appointment and its associated products and salesperson.
        /// </summary>
        /// <param name="id">The id of the appointment to retrieve.</param>
        /// <returns>The view containing the appointment details.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return NotFound();

            var products = (from p in _db.Products
                            join a in _db.ProductsForAppointments
                            on p.Id equals a.ProductId
                            where a.AppointmentId == id
                            select p).Include("ProductTypes") as IEnumerable<Product>;

            AppointmentDetailsViewModel detailsVM = new()
            {
                Appointment = await _db.Appointments.Include(x => x.SalesPerson).FirstOrDefaultAsync(x => x.Id == id),
                SalesPerson = await _db.ApplicationUsers.ToListAsync(),
                Products = products.ToList()
            };

            detailsVM.Appointment.AppointmentTime = detailsVM.Appointment.AppointmentTime
                                                             .AddHours(detailsVM.Appointment.AppointmentDay.Hour)
                                                             .AddMinutes(detailsVM.Appointment.AppointmentDay.Minute);

            return View(detailsVM);
        }

        /// <summary>
        /// Edit an existing appointment in the database
        /// </summary>
        /// <param name="id">The id of the appointment to be edited</param>
        /// <param name="currentAppointment">The new appointment details</param>
        /// <returns>Redirects to the Index page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel currentAppointment)
        {
            if (!ModelState.IsValid)
                return View(currentAppointment);
            if (!id.Equals(currentAppointment.Appointment.Id))
                return NotFound();

            currentAppointment.Appointment.AppointmentDay = currentAppointment.Appointment.AppointmentDay
                                                                              .AddHours(currentAppointment.Appointment.AppointmentTime.Hour)
                                                                              .AddMinutes(currentAppointment.Appointment.AppointmentTime.Minute);

            var appointmentFromDB = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == currentAppointment.Appointment.Id);

            appointmentFromDB.CustomerFirstName = currentAppointment.Appointment.CustomerFirstName;
            appointmentFromDB.CustomerLastName = currentAppointment.Appointment.CustomerLastName;
            appointmentFromDB.Adress = currentAppointment.Appointment.Adress;
            appointmentFromDB.Index = currentAppointment.Appointment.Index;
            appointmentFromDB.City = currentAppointment.Appointment.City;
            appointmentFromDB.CustomerEmail = currentAppointment.Appointment.CustomerEmail;
            appointmentFromDB.CustomerPhoneNumber = currentAppointment.Appointment.CustomerPhoneNumber;
            appointmentFromDB.AppointmentDay = currentAppointment.Appointment.AppointmentDay;
            appointmentFromDB.IsConfirmed = currentAppointment.Appointment.IsConfirmed;

            if (User.IsInRole(SD.SuperAdminEndUser))
                appointmentFromDB.SalesPersonId = currentAppointment.Appointment.SalesPersonId;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retrieves the details of an appointment, including the associated products and salesperson.
        /// </summary>
        /// <param name="id">The ID of the appointment to retrieve.</param>
        /// <returns>The view containing the appointment details.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return NotFound();

            var products = (from p in _db.Products
                            join a in _db.ProductsForAppointments
                            on p.Id equals a.ProductId
                            where a.AppointmentId == id
                            select p).Include("ProductTypes") as IEnumerable<Product>;

            AppointmentDetailsViewModel detailsVM = new()
            {
                Appointment = await _db.Appointments.Include(x => x.SalesPerson).FirstOrDefaultAsync(x => x.Id == id),
                SalesPerson = await _db.ApplicationUsers.ToListAsync(),
                Products = products.ToList()
            };

            detailsVM.Appointment.AppointmentTime = detailsVM.Appointment.AppointmentTime
                                                             .AddHours(detailsVM.Appointment.AppointmentDay.Hour)
                                                             .AddMinutes(detailsVM.Appointment.AppointmentDay.Minute);

            return View(detailsVM);
        }

        /// <summary>
        /// Retrieves the details of an appointment, including the associated products and salesperson.
        /// </summary>
        /// <param name="id">The ID of the appointment to retrieve.</param>
        /// <returns>A view containing the details of the appointment.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return NotFound();

            var products = (from p in _db.Products
                            join a in _db.ProductsForAppointments
                            on p.Id equals a.ProductId
                            where a.AppointmentId == id
                            select p).Include("ProductTypes") as IEnumerable<Product>;

            AppointmentDetailsViewModel detailsVM = new()
            {
                Appointment = await _db.Appointments.Include(x => x.SalesPerson).FirstOrDefaultAsync(x => x.Id == id),
                SalesPerson = await _db.ApplicationUsers.ToListAsync(),
                Products = products.ToList()
            };

            detailsVM.Appointment.AppointmentTime = detailsVM.Appointment.AppointmentTime
                                                             .AddHours(detailsVM.Appointment.AppointmentDay.Hour)
                                                             .AddMinutes(detailsVM.Appointment.AppointmentDay.Minute);

            return View(detailsVM);
        }

        /// <summary>
        /// Deletes an appointment from the database.
        /// </summary>
        /// <param name="id">The id of the appointment to delete.</param>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var appointmentFromDB = await _db.Appointments.FindAsync(id);

            _db.Appointments.Remove(appointmentFromDB);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}