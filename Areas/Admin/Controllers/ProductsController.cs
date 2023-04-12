using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using CoreStoreMVC.Models.ViewModel;
using CoreStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area(nameof(Admin))]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ProductsVM = new ProductsViewModel()
            {
                Products = new Product(),
                ProductTypes = _db.ProductsTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList()
            };
        }

        /// <summary>
        /// Retrieves a list of products from the database and passes them to the view.
        /// </summary>
        /// <returns>A view containing a list of products.</returns>
        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(x => x.ProductTypes).Include(x => x.SpecialTags);
            return View(await products.ToListAsync());
        }

        /// <summary>
        /// Retrieves the view for creating a new product.
        /// </summary>
        /// <returns>The view for creating a new product.</returns>
        [HttpGet]
        public IActionResult Create() => View(ProductsVM);

        /// <summary>
        /// Creates a new product and saves it to the database.
        /// </summary>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
                return View(ProductsVM);

            await _db.Products.AddAsync(ProductsVM.Products);
            await _db.SaveChangesAsync();

            string webRootPath = _hostingEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            var productsFromDb = await _db.Products.FindAsync(ProductsVM.Products.Id);
            if (files.Count != 0)
            {
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploadPath, ProductsVM.Products.Id + extension), FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }

                productsFromDb.Image = $"\\{SD.ImageFolder}\\{ProductsVM.Products.Id}{extension}";
            }
            else
            {
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploadPath, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png");
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png";
            }

            await _db.SaveChangesAsync();
            TempData["SM"] = $"Product {ProductsVM.Products.Name} adding successful!";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retrieves the product with the specified id and returns it in a view.
        /// </summary>
        /// <param name="id">The id of the product to be retrieved.</param>
        /// <returns>The view containing the product with the specified id.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            ProductsVM.Products = await _db.Products
                                           .Include(x => x.ProductTypes)
                                           .Include(x => x.SpecialTags)
                                           .FirstOrDefaultAsync(x => x.Id == id);

            if (ProductsVM.Products is null)
                return NotFound();

            return View(ProductsVM);
        }

        // POST: Products/Edit
        /// <summary>
        /// This method updates a product in the database.
        /// </summary>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (!ModelState.IsValid)
                return View(ProductsVM);

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productFromDb = _db.Products.FirstOrDefault(x => x.Id == ProductsVM.Products.Id);
            if (files.Count > 0 && files[0] != null)
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension_new = Path.GetExtension(files[0].FileName);
                var extension_old = Path.GetExtension(productFromDb.Image);
                if (System.IO.File.Exists(Path.Combine(uploads, ProductsVM.Products.Id + extension_old)))
                    System.IO.File.Delete(Path.Combine(uploads, ProductsVM.Products.Id + extension_old));

                using (var fileStream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension_new), FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }
                ProductsVM.Products.Image = $"\\{SD.ImageFolder}\\{ProductsVM.Products.Id}{extension_new}";
            }

            if (ProductsVM.Products.Image != null)
                productFromDb.Image = ProductsVM.Products.Image;

            productFromDb.Name = ProductsVM.Products.Name;
            productFromDb.Price = ProductsVM.Products.Price;
            productFromDb.Available = ProductsVM.Products.Available;
            productFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
            productFromDb.SpecialTagId = ProductsVM.Products.SpecialTagId;
            productFromDb.ShadeColor = ProductsVM.Products.ShadeColor;

            await _db.SaveChangesAsync();
            TempData["SM"] = $"Product {ProductsVM.Products.Name} update successful!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Details
        /// <summary>
        /// Retrieves the details of a product from the database.
        /// </summary>
        /// <param name="id">The id of the product to be retrieved.</param>
        /// <returns>The view of the product details.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            ProductsVM.Products = await _db.Products
                                           .Include(x => x.ProductTypes)
                                           .Include(x => x.SpecialTags)
                                           .FirstOrDefaultAsync(x => x.Id == id);

            if (ProductsVM.Products is null)
                return NotFound();

            return View(ProductsVM);
        }

        // GET: Products/Delete
        /// <summary>
        /// Retrieves a product from the database and returns it in a view.
        /// </summary>
        /// <param name="id">The id of the product to be retrieved.</param>
        /// <returns>The view containing the product.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            ProductsVM.Products = await _db.Products
                                           .Include(x => x.ProductTypes)
                                           .Include(x => x.SpecialTags)
                                           .FirstOrDefaultAsync(x => x.Id == id);

            if (ProductsVM.Products is null)
                return NotFound();

            return View(ProductsVM);
        }

        // POST: Products/Delete
        /// <summary>
        /// Deletes a product from the database.
        /// </summary>
        /// <param name="id">The id of the product to be deleted.</param>
        /// <returns>Redirects to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Product product = await _db.Products.FindAsync(id);

            if (product == null)
                return NotFound();
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(product.Image);

                if (System.IO.File.Exists(Path.Combine(uploads, product.Id + extension)))
                    System.IO.File.Delete(Path.Combine(uploads, product.Id + extension));

                _db.Products.Remove(product);
                await _db.SaveChangesAsync();

                TempData["SM"] = $"Product: {product.Name} deleting successful!";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
