#region

using Volo.Abp.Application.Dtos;

#endregion

namespace Logiwa.Products
{
    public class GetProductsInput : PagedAndSortedResultRequestDto
    {
        public string Filters { get; set; }

    }
}