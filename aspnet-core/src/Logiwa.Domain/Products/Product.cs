#region

using System;
using System.ComponentModel.DataAnnotations;
using Logiwa.Codes;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;

#endregion

namespace Logiwa.Products
{
    public class Product : FullAuditedAggregateRoot<Guid>
    {
        public Product()
        {
            
        }

        public Product(Guid id,string code, string title, string description,int stockQuantity,decimal price,bool isLive,Guid? parentId)
        {
            Id = id;
            Code = new Code(code);
            Title = title;
            Description = description;
            StockQuantity = stockQuantity;
            Price = price;
            IsLive = isLive;
            ParentId = parentId;
        }
        
        public virtual Code Code { get; set; }
        
        public virtual string Title { get; set; }
        
        [CanBeNull]
        public virtual string Description { get; set; }
        public virtual int StockQuantity { get; set; }
        public virtual decimal Price { get; set; }
        public virtual bool IsLive { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ParentId { get; set; }
    }
}