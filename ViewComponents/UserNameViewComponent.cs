using System.Security.Claims;
using System.Threading.Tasks;
using CoreStoreMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreStoreMVC.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves the user from the database based on the user's identity.
        /// </summary>
        /// <returns>
        /// The view component result containing the user.
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == claims.Value);

            return View(userFromDb);
        }
    }
}
