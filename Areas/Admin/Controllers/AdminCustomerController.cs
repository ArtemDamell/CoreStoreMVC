using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CoreStoreMVC.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class AdminCustomerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminCustomerController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a list of ClientUsers from the database and returns it to the view.
        /// </summary>
        /// <returns>
        /// A view containing a list of ClientUsers.
        /// </returns>
        public async Task<IActionResult> Index()
        {
            var userList = await _db.ClientUsers.ToListAsync();
            return View(userList);
        }

        /// <summary>
        /// Bans a user by their ID and adds their IP to the blacklist.
        /// </summary>
        /// <param name="id">The ID of the user to ban.</param>
        /// <returns>A redirect to the Index action.</returns>
        public async Task<IActionResult> BanById(string id)
        {
            var userFromDb = await _db.ClientUsers.FindAsync(id);

            if (userFromDb is null)
                return NotFound();

            // Take user banned by 1000 year
            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

            // Save in db IP in black list
            IpBlackList ipForList = new()
            {
                Address = userFromDb.UserIP,
                UserId = id
            };

            await _db.IpBlackLists.AddAsync(ipForList);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}