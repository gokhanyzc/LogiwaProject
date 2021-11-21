#region

using Logiwa.Categories;

#endregion

namespace Logiwa.Products
{
    public class ProductWithNavigationProperties
    {
        public Product Product { get; set; }
        public Category Category { get; set; }
    }
}