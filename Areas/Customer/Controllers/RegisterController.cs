using System;
using System.Linq;
using System.Management;
using System.Net;
using System.Threading.Tasks;
using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using CoreStoreMVC.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreStoreMVC.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public RegisterController(ApplicationDbContext db, UserManager<IdentityUser> userManager,
                                  RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public RegistrationUserModel Input { get; set; }
        public UserLoginModel InputLogin { get; set; }

        /// <summary>
        /// Retrieves the Register view.
        /// </summary>
        /// <returns>The Register view.</returns>
        [HttpGet]
        public IActionResult Register() => View();

        /// <summary>
        /// This method handles the registration of a new user. It checks if the user's IP is locked, if the IP or MAC is not exists, and if the user's credentials are valid. If all checks pass, the user is added to the database and redirected to the home page.
        /// </summary>
        /// <returns>Redirects to the home page if successful, otherwise returns the registration view.</returns>
        [HttpPost, ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPost(RegistrationUserModel input)
        {
            if (!ModelState.IsValid)
                return View(Input);

            var userIP = GetUserIPOrNull();
            var userMAC = GetUserMACOrNull();
            var res = await IpIsLocked(userIP);
            if (res)
            {
                ModelState.AddModelError("", "Your IP IS BANNED! Good Bye!");
                return View(Input);
            }

            if (string.IsNullOrWhiteSpace(userIP) || string.IsNullOrWhiteSpace(userMAC))
            {
                ModelState.AddModelError("", "IP or MAC is not exists! Contact the admin, please!");
                return View(Input);
            }

            var user = new ClientUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.Name,
                PhoneNumber = Input.PhoneNumber,
                UserIP = userIP,
                UserMAC = userMAC
            };

            var creationResult = await _userManager.CreateAsync(user, Input.Password);
            if (creationResult.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(SD.UserEndRole))
                    await _roleManager.CreateAsync(new IdentityRole(SD.UserEndRole));

                await _userManager.AddToRoleAsync(user, SD.UserEndRole);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in creationResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        /// <summary>
        /// Retrieves the Login view.
        /// </summary>
        /// <returns>The Login view.</returns>
        [HttpGet]
        public IActionResult Login() => View();

        /// <summary>
        /// Logs in a user with the given credentials.
        /// </summary>
        /// <param name="inputLogin">The user's login credentials.</param>
        /// <returns>
        /// Redirects to the home page if the login is successful, otherwise returns the login page with an error message.
        /// </returns>
        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPost(UserLoginModel inputLogin)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(InputLogin);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(inputLogin.Email, inputLogin.Password, inputLogin.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Login FAILED!");
            return View(InputLogin);
        }

        /// <summary>
        /// Gets the IP address of the current user or returns null if not found.
        /// </summary>
        /// <returns>The IP address of the current user or null.</returns>
        private string GetUserIPOrNull()
        {
            IPHostEntry hostIPInfo = Dns.GetHostEntry(Dns.GetHostName());
            string currentIP = Convert.ToString(hostIPInfo.AddressList.FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork));
            return (!string.IsNullOrWhiteSpace(currentIP)) ? currentIP : null;
        }

        /// <summary>
        /// Gets the MAC address of the user's network adapter.
        /// </summary>
        /// <returns>The MAC address of the user's network adapter, or null if not found.</returns>
        private string GetUserMACOrNull()
        {
            ManagementClass mc = new("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            string MACAddress = string.Empty;
            foreach (var mo in moc)
            {
                if (string.IsNullOrEmpty(MACAddress))
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        MACAddress = mo["MacAddress"].ToString();
                        break;
                    }
                }
                mo.Dispose();
            }

            return (!string.IsNullOrEmpty(MACAddress)) ? MACAddress : null;
        }

        /// <summary>
        /// Checks if the given IP address is locked.
        /// </summary>
        /// <param name="userIp">The IP address to check.</param>
        /// <returns>True if the IP address is locked, false otherwise.</returns>
        private async Task<bool> IpIsLocked(string userIp)
        {
            var blockedIp = await _db.IpBlackLists.FirstOrDefaultAsync(x => x.Address == userIp);
            return (blockedIp is null) ? false : true;
        }
    }
}
