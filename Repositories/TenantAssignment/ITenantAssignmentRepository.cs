using Stripe;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.TenantAssignment
{
    public interface ITenantAssignmentRepository
    {

        Task<IEnumerable<TenantAssignmentGetDto>> GetByCompanyAllId(int companyId, int? unitId = null);
        Task<object> GetByCompanyIdAsync(int companyId, bool isBedSpace = false, DateTime? startDate = null, DateTime? endDate = null, int? propertyId = null, int? unitId = null, string? search = null, int pageNumber = 1, int pageSize = 25);
        Task<object> GeClosure( int companyId, DateTime? startDate = null, DateTime? endDate = null, int? propertyId = null, int? unitId = null, string? search = null, int pageNumber = 1, int pageSize = 25);
        Task<TenantAssignmentGetDto?> GetClosureById(int tenantAssignId);
        Task<TenantAssignmentGetDto?> GetByIdAsync(int tenantAssignId);
        Task<XRS_TenantAssignment> CreateAsync(TenantAssignmentCreateDto dto);
        Task<bool> UpdateAsync(int tenantAssignId, TenantAssignmentCreateDto dto);
        Task<bool> UpdateClosureAsync(int tenantAssignId, TenantClosureCreateDto dto);
        Task<bool> DeleteAsync(int tenantAssignId);
        Task<object> GetChequesByCompanyAsync( int companyId, string? search = null, DateTime? startDate = null, DateTime? endDate = null, string? status = null, int pageNumber = 1, int pageSize = 25);
        Task<bool> UpdateChequePayStatusAsync(int chequeRegisterId, string payStatus);
    }
}
