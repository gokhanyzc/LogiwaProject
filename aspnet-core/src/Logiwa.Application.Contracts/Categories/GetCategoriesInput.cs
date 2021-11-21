#region

using Volo.Abp.Application.Dtos;

#endregion

namespace Logiwa.Categories
{
    public class GetCategoriesInput : PagedAndSortedResultRequestDto
    {
        public string Filters { get; set; }
    }
}