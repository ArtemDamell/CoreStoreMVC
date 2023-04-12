using System.Linq;
using System.Threading.Tasks;
using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using CoreStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreStoreMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area(nameof(Admin))]
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a list of SpecialTags from the database and returns it to the View.
        /// </summary>
        public IActionResult Index() => View(_db.SpecialTags.ToList());

        /// <summary>
        /// Gets the Create view.
        /// </summary>
        /// <returns>The Create view.</returns>
        [HttpGet]
        public IActionResult Create() => View();

        /// <summary>
        /// Creates a new SpecialTag object and adds it to the database.
        /// </summary>
        /// <param name="specialTag">The SpecialTag object to be added.</param>
        /// <returns>Redirects to the Index page if successful, otherwise returns the view with the SpecialTag object.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                _db.Add(specialTag);
                await _db.SaveChangesAsync();

                TempData["SM"] = $"Special tag: {specialTag.Name} added successful!";

                return RedirectToAction(nameof(Index));
            }
            return View(specialTag);
        }

        /// <summary>
        /// Retrieves the SpecialTag with the specified id for editing.
        /// </summary>
        /// <param name="id">The id of the SpecialTag to be edited.</param>
        /// <returns>The view for editing the SpecialTag.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);
            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        /// <summary>
        /// Edits a special tag in the database.
        /// </summary>
        /// <param name="id">The id of the special tag to be edited.</param>
        /// <param name="specialTag">The updated special tag.</param>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTag specialTag)
        {
            if (id != specialTag.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(specialTag);

            _db.Update(specialTag);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Special tag {specialTag.Name} editing successful.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retrieves the details of a SpecialTag from the database.
        /// </summary>
        /// <param name="id">The id of the SpecialTag to retrieve.</param>
        /// <returns>The view of the SpecialTag details.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);
            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        /// <summary>
        /// Retrieves the SpecialTag with the specified id and returns it in a view.
        /// </summary>
        /// <param name="id">The id of the SpecialTag to be retrieved.</param>
        /// <returns>The view containing the SpecialTag with the specified id.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        /// <summary>
        /// Deletes a special tag from the database.
        /// </summary>
        /// <param name="id">The id of the special tag to be deleted.</param>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialTag = await _db.SpecialTags.FindAsync(id);
            if (specialTag != null)
            {
                _db.SpecialTags.Remove(specialTag);
                await _db.SaveChangesAsync();

                TempData["SM"] = $"Special tag {specialTag.Name} deleting successful.";
            }
            else
                TempData["SM"] = $"Special tag {specialTag.Name} deleting FAILED.";

            return RedirectToAction(nameof(Index));
        }
    }
}
