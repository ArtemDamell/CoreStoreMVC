using System;
using System.Threading.Tasks;
using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using CoreStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreStoreMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area(nameof(Admin))]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a list of ApplicationUsers from the database and returns it to the view.
        /// </summary>
        public async Task<IActionResult> Index() => View(await _db.ApplicationUsers.ToListAsync());

        /// <summary>
        /// Retrieves the user with the specified ID from the database and returns it to the view.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The view with the retrieved user.</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id is null || id.Trim().Length is 0)
                return NotFound();

            var userFromDb = await _db.ApplicationUsers.FindAsync(id);

            if (userFromDb is null)
                return NotFound();

            return View(userFromDb);
        }

        /// <summary>
        /// Updates an existing ApplicationUser in the database.
        /// </summary>
        /// <param name="id">The ID of the ApplicationUser to update.</param>
        /// <param name="applicationUser">The ApplicationUser object containing the updated information.</param>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(applicationUser);

            ApplicationUser userFromDb = await _db.ApplicationUsers.FindAsync(id);
            userFromDb.Name = applicationUser.Name;
            userFromDb.PhoneNumber = applicationUser.PhoneNumber;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retrieves an ApplicationUser from the database based on the given id.
        /// </summary>
        /// <param name="id">The id of the ApplicationUser to retrieve.</param>
        /// <returns>The ApplicationUser with the given id, or NotFound if the user does not exist.</returns>
        public async Task<IActionResult> Delete(string id)
        {
            if (id is null || id.Trim().Length is 0)
                return NotFound();

            var userFromDb = await _db.ApplicationUsers.FindAsync(id);

            if (userFromDb is null)
                return NotFound();

            return View(userFromDb);
        }

        /// <summary>
        /// This method sets the lockout end date of the specified user to 1000 years in the future.
        /// </summary>
        /// <param name="id">The id of the user to be locked out.</param>
        /// <returns>Redirects to the Index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string id)
        {
            ApplicationUser userFromDb = await _db.ApplicationUsers.FindAsync(id);

            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
