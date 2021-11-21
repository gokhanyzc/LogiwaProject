#region

using System;
using Volo.Abp.Application.Dtos;

#endregion

namespace Logiwa.Products
{
    public class ProductDto : FullAuditedEntityDto<Guid>
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public bool IsLive { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ParentId { get; set; }
    }
}