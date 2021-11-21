#region

using System;
using Volo.Abp.Application.Dtos;

#endregion

namespace Logiwa.Categories
{
    public class CategoryDto : FullAuditedEntityDto<Guid>
    {
        public string Code { get; set; }
        public string CategoryName { get; set; }
        public int MinStockQuantity { get; set; }
        public int FirstQuantity { get; set; }
        public Guid? ParentId { get; set; }
    }
}