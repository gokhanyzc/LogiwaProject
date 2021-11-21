#region

using Volo.Abp.Application.Dtos;

#endregion


namespace Logiwa.Shared
{
    public class LookupRequestDto : PagedResultRequestDto
    {
        public string Filter { get; set; }
    }
}