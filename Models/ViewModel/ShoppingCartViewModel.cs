using System.Collections.Generic;

namespace CoreStoreMVC.Models.ViewModel
{
    public class ShoppingCartViewModel
    {
        public List<Product> Products { get; set; }
        public Appointment Appointment { get; set; }
        public PayPalConfig PayPalConfig { get; set; }
    }
}
