namespace XeniaTenoraBackend.Service.Socket
{

    public interface ITenoraUpdateService
    {
        Task SendTenoraUpdate(int companyId, int? tenanatId = null, int? id = null, int? pageNumber = null, int? pageSize = null, string? search = null, string? connectionId = null);
    }
}
