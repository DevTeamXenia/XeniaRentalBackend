namespace XeniaTenoraBackend.Service.Socket
{
    public interface ITenoraUpdateService
    {
        Task SendTenoraUpdate(
            int companyId,
            int? tenantId = null,
            int? employeeId = null,
            int? pageNumber = null,
            int? pageSize = null,
            string? search = null,
            string? status = null,
            string? connectionId = null
        );
    }
}