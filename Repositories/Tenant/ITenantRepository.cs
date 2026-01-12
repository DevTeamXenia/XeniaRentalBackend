using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Tenant
{
    public interface ITenantRepository
    {
        Task<IEnumerable<XRS_Tenant>> GetTenants(int companyId, int? unitId = null);
        Task<PagedResultDto<TenantGetDto>> GetTenantsByCompanyId(int companyId, bool? status = null, string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<TenantGetDto> GetTenantWithDocumentsById(int tenantId);
        Task<TenantProfileDto> GetProfileById();
        Task<XRS_Tenant> CreateTenant(TenantCreateDto tenantDto);
        Task<bool> UpdateTenant(int tenantId, TenantCreateDto tenantDto);
        Task<Dictionary<string, string>> UploadFilesAsync(List<IFormFile> files);
        Task<(byte[] FileContent, string ContentType)?> GetImageFromFtpAsync(string fileName);
        Task<bool> DeleteTenant(int id);



    }
}
