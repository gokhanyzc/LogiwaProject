#region

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace Logiwa.Products
{
    public class ProductUpdateDto
    {
        private string _title;
        private string _description;
       
        [Required(ErrorMessage = LogiwaDomainErrorCodes.ProductTitleNullCheck)]
        [MaxLength(200,ErrorMessage = LogiwaDomainErrorCodes.ProductTitleCharacterLimitControl)] 
        public string Title
        {
            get => _title;
            set => _title = value.TrimProperty();
        }
        
        public string Description
        {
            get => _description;
            set => _description = value.TrimProperty();
        }
        
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
    //  public bool IsLive { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ParentId { get; set; }

    }
}