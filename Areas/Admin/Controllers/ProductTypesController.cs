using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using CoreStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area(nameof(Admin))]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a list of all product types from the database and returns it to the view. 
        /// </summary>
        public IActionResult Index() => View(_db.ProductsTypes.ToList());

        /// <summary>
        /// Gets the Create view.
        /// </summary>
        /// <returns>The Create view.</returns>
        [HttpGet]
        public IActionResult Create() => View();

        /// <summary>
        /// Creates a new product type and adds it to the database.
        /// </summary>
        /// <param name="productsType">The product type to be added.</param>
        /// <returns>Redirects to the Index page if successful, otherwise returns the view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsType productsType)
        {
            if (ModelState.IsValid)
            {
                _db.Add(productsType);
                await _db.SaveChangesAsync();

                TempData["SM"] = $"Product type: {productsType.Name} added successful!";

                return RedirectToAction(nameof(Index));
            }
            return View(productsType);
        }

        /// <summary>
        /// Retrieves the product type with the specified id for editing.
        /// </summary>
        /// <param name="id">The id of the product type to be edited.</param>
        /// <returns>The view for editing the product type.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var productType = await _db.ProductsTypes.FindAsync(id);
            if (productType == null)
                return NotFound();

            return View(productType);
        }

        /// <summary>
        /// Edits the specified product type.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="productsType">Type of the products.</param>
        /// <returns>
        /// View of the edited product type.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductsType productsType)
        {
            if (id != productsType.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(productsType);

            _db.Update(productsType);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Product type {productsType.Name} editing successful.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retrieves the details of a product type from the database.
        /// </summary>
        /// <param name="id">The id of the product type to retrieve.</param>
        /// <returns>The view of the product type details.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var productType = await _db.ProductsTypes.FindAsync(id);
            if (productType == null)
                return NotFound();

            return View(productType);
        }

        /// <summary>
        /// Retrieves a product type from the database based on the given id.
        /// </summary>
        /// <param name="id">The id of the product type to be retrieved.</param>
        /// <returns>The view of the product type.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var productType = await _db.ProductsTypes.FindAsync(id);

            if (productType == null)
                return NotFound();

            return View(productType);
        }

        /// <summary>
        /// Deletes a product type from the database.
        /// </summary>
        /// <param name="id">The id of the product type to be deleted.</param>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productType = await _db.ProductsTypes.FindAsync(id);
            if (productType != null)
            {
                _db.ProductsTypes.Remove(productType);
                await _db.SaveChangesAsync();

                TempData["SM"] = $"Product type {productType.Name} deleting successful.";
            }
            else
                TempData["SM"] = $"Product type {productType.Name} deleting FAILED.";

            return RedirectToAction(nameof(Index));
        }
    }
}