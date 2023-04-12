using Microsoft.AspNetCore.Identity;

namespace CoreStoreMVC.Models
{
    public class ClientUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string UserIP { get; set; }
        public string UserMAC { get; set; }
    }
}
