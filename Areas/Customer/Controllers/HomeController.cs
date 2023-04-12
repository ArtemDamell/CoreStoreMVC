using Microsoft.AspNetCore.Mvc;
using CoreStoreMVC.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using CoreStoreMVC.Extensions;

namespace CoreStoreMVC.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a list of products from the database and returns them to the view.
        /// </summary>
        /// <returns>A view containing a list of products.</returns>
        public async Task<IActionResult> Index()
        {
            var productList = await _db.Products.Include(x => x.ProductTypes)
                                                .Include(x => x.SpecialTags)
                                                .ToListAsync();

            return View(productList);
        }

        // GET: Index/Details/Id
        /// <summary>
        /// Retrieves the details of a product with the given id.
        /// </summary>
        /// <param name="id">The id of the product to retrieve.</param>
        /// <returns>The details of the product, or a NotFound result if the product does not exist.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.Include(x => x.ProductTypes)
                                            .Include(x => x.SpecialTags)
                                            .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
                return NotFound();

            return View(product);
        }

        // POST: Index/Details/Id
        /// <summary>
        /// Adds the specified item to the shopping cart.
        /// </summary>
        /// <param name="id">The ID of the item to add.</param>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost(int id)
        {
            List<int> listOfShoppingCart = HttpContext.Session.Get<List<int>>("sShoppingCart");
            if (listOfShoppingCart is null)
                listOfShoppingCart = new List<int>();

            listOfShoppingCart.Add(id);

            HttpContext.Session.Set("sShoppingCart", listOfShoppingCart);

            return RedirectToAction(nameof(Index));
        }

        // GET: Index/Remove/Id
        /// <summary>
        /// Removes a product from the shopping cart.
        /// </summary>
        /// <param name="id">The id of the product to be removed.</param>
        /// <returns>Redirects to the Index action.</returns>
        public IActionResult Remove(int id)
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("sShoppingCart");
            if (listShoppingCart.Count > 0)
            {
                if (listShoppingCart.Contains(id))
                    listShoppingCart.Remove(id);
            }

            HttpContext.Session.Set("sShoppingCart", listShoppingCart);

            TempData["SM"] = "Product removed from your cart";

            return RedirectToAction(nameof(Index));
        }
    }
}