



using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Dtos.XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.AdminComplaint
{
    public interface IAdminComplaintRepository
    {
        // Tabs
        Task<PagedResultDto<object>> GetNewComplaints(
            int companyId, string? search = null,
            string? zone = null,
            int pageNumber = 1, int pageSize = 10);

        Task<PagedResultDto<object>> GetInProgressComplaints(
            int companyId, string? search = null,
            string? zone = null,
            int pageNumber = 1, int pageSize = 10);

        Task<PagedResultDto<object>> GetClosedComplaints(
            int companyId, string? search = null,
            string? zone = null,
            int pageNumber = 1, int pageSize = 10);

        Task<PagedResultDto<object>> GetOverdueComplaints(
            int companyId, string? search = null,
            string? zone = null,
            int pageNumber = 1, int pageSize = 10);

        // Assign Modal
        Task<XRS_ComplaintAssignment> AssignComplaint(ComplaintAssignmentDto dto);

        // Complaint Details — Grid Click
        Task<object?> GetComplaintDetails(int maintenanceId);
        // IAdminComplaintRepository.cs-ൽ Add ചെയ്യുക
        Task<PagedResultDto<object>> GetMaintenanceReport(
            int companyId,
            string? search = null,
            string? status = null,
            string? zone = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            int pageNumber = 1,
            int pageSize = 10);
    }
}