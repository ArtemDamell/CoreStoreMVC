using CoreStoreMVC.Data;
using CoreStoreMVC.Extensions;
using CoreStoreMVC.Models;
using CoreStoreMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;

            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Product>()
            };
        }

        // GET Shopping Cart
        /// <summary>
        /// Retrieves the products in the shopping cart and adds them to the ShoppingCartVM.
        /// </summary>
        /// <returns>View of ShoppingCartVM</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sShoppingCart");

            if (shoppingCartList is not null && shoppingCartList.Count > 0)
            {
                foreach (var cartItem in shoppingCartList)
                {
                    Product product = await _db.Products.Include(x => x.SpecialTags)
                                                  .Include(x => x.ProductTypes)
                                                  .Where(x => x.Id == cartItem)
                                                  .FirstOrDefaultAsync();

                    ShoppingCartVM.Products.Add(product);
                }

            }

            return View(ShoppingCartVM);
        }

        /// <summary>
        /// Creates an appointment and adds the products in the shopping cart to the appointment.
        /// </summary>
        /// <returns>Redirects to the AppointmentConfirmation action.</returns>
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexPost()
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sShoppingCart");

            ShoppingCartVM.Appointment.AppointmentDay = ShoppingCartVM.Appointment.AppointmentDay
                                                                      .AddHours(ShoppingCartVM.Appointment.AppointmentTime.Hour)
                                                                      .AddMinutes(ShoppingCartVM.Appointment.AppointmentTime.Minute);

            Appointment appointment = ShoppingCartVM.Appointment;

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            int appointmentId = appointment.Id;

            foreach (var item in shoppingCartList)
            {
                ProductsForAppointment productsForAppointment = new ProductsForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = item
                };
                _db.ProductsForAppointments.Add(productsForAppointment);
            }
            await _db.SaveChangesAsync();

            shoppingCartList = new List<int>();
            HttpContext.Session.Set("sShoppingCart", shoppingCartList);

            return RedirectToAction(nameof(AppointmentConfirmation), new { Id = appointmentId });
        }

        /// <summary>
        /// Removes an item from the shopping cart.
        /// </summary>
        /// <param name="id">The id of the item to be removed.</param>
        /// <returns>Redirects to the Index action.</returns>
        public IActionResult Remove(int id)
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sShoppingCart");

            if (shoppingCartList.Count > 0)
            {
                if (shoppingCartList.Contains(id))
                    shoppingCartList.Remove(id);
            }

            HttpContext.Session.Set("sShoppingCart", shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retrieves the appointment and associated products for the given appointment ID and returns the ShoppingCartVM view.
        /// </summary>
        /// <param name="id">The ID of the appointment.</param>
        /// <returns>The ShoppingCartVM view.</returns>
        [HttpGet]
        public async Task<IActionResult> AppointmentConfirmation(int id)
        {
            ShoppingCartVM.Appointment = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            List<ProductsForAppointment> productListObj = await _db.ProductsForAppointments.Where(x => x.AppointmentId == id).ToListAsync();

            foreach (var item in productListObj)
            {
                item.Quantity = 1;
                ShoppingCartVM.Products.Add(await _db.Products
                                                     .Include(x => x.ProductTypes)
                                                     .Include(x => x.SpecialTags)
                                                     .FirstOrDefaultAsync(x => x.Id == item.ProductId));
            }
            ShoppingCartVM.PayPalConfig = PayPal.GetPayPalConfig();

            return View(ShoppingCartVM);
        }

        /// <summary>
        /// This method is used to process a successful payment and return a view with the result.
        /// </summary>
        /// <returns>A view with the result of the successful payment.</returns>
        public IActionResult Success()
        {
            var result = PDTHolder.Success(Request.Query["tx"].ToString());

            return View(result);
        }
    }
}
