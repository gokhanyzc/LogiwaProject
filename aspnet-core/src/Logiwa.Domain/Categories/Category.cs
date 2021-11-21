#region

using System;
using System.ComponentModel.DataAnnotations;
using Logiwa.Codes;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;

#endregion


namespace Logiwa.Categories
{
    public class Category : FullAuditedAggregateRoot<Guid>
    {
        public Category()
        {
            
        }

        public Category(Guid id,string code,string categoryName,int minStockQuantity, int firstQuantity,Guid? parentId)
        {
            Id = id;
            Code = new Code(code);
            CategoryName = categoryName;
            ParentId = parentId;
            MinStockQuantity = minStockQuantity;
            FirstQuantity = firstQuantity;
        }
        
        public virtual Code Code { get; set; }
        
        [CanBeNull]
        public virtual string CategoryName { get; set; }
        public virtual int MinStockQuantity { get; set; }
        public virtual int FirstQuantity { get; set; }
        public Guid? ParentId { get; set; }

        
        
        
    }
}