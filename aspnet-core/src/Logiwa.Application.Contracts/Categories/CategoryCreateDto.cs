#region

using System;

#endregion

namespace Logiwa.Categories
{
    public class CategoryCreateDto
    {
        private string _categoryName;

        public string CategoryName
        {
            get => _categoryName;
            set => _categoryName = value.TrimProperty();
        }
        public int MinStockQuantity { get; set; } 
        public Guid? ParentId { get; set; }

    }
}