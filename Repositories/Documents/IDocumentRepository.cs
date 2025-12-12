using XeniaRentalApi.Dtos;
using XeniaRentalApi.Models;

namespace XeniaRentalApi.Repositories.Documents
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<XRS_Documents>> GetDocuments(int companyId);
        Task<PagedResultDto<XRS_Documents>> GetDocumentsCompanyId(int companyId, string? search = null, string? docPurpose = null, int pageNumber = 1, int pageSize = 10);

        Task<XRS_Documents> CreateDocuments(XRS_Documents documents);

        Task<bool> DeleteDocumentType(int id);

        Task<XRS_Documents> GetDocumentById(int documentId);

        Task<bool> UpdateDocuments(int id, XRS_Documents documents);

    }
}
