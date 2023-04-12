using System.Collections.Generic;

namespace CoreStoreMVC.Models.ViewModel
{
    public class ProductsViewModel
    {
        public Product Products { get; set; }
        public IEnumerable<ProductsType> ProductTypes { get; set; }
        public IEnumerable<SpecialTag> SpecialTags { get; set; }
    }
}
