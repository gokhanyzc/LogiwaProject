#region

using Logiwa.Categories;

#endregion

namespace Logiwa.Products
{
    public class ProductWithNavigationPropertiesDto
    {
        public ProductDto Product { get; set; }
        public CategoryDto Category { get; set; }
    }
}