using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Documents
{
    public class DocumentRepository:IDocumentRepository
    {
        private readonly ApplicationDbContext _context;
        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<XRS_Documents>> GetDocuments(int companyId)
        {
            return await _context.Documents
                .Where(u => u.companyID == companyId)
                .Select(u => new Models.XRS_Documents
                {
                    docTypeId = u.docTypeId,
                    docPurpose = u.docPurpose,
                    docName = u.docName,            
                    isActive = u.isActive,
                    isMandatory = u.isMandatory,
                    isAlphanumeric = u.isAlphanumeric,
                    isExpiry = u.isExpiry,
                    ExpiryDate = u.ExpiryDate
                })
                .ToListAsync();
        }

        public async Task<PagedResultDto<XRS_Documents>> GetDocumentsCompanyId(int companyId, string? search = null, string? docPurpose = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Documents
                .Where(u => u.companyID == companyId); 
      
            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(u => u.docName.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrWhiteSpace(docPurpose))
            {
                query = query.Where(u => u.docPurpose.ToLower() == docPurpose.ToLower());
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.docName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new XRS_Documents
                {
                    docTypeId = u.docTypeId,
                    docPurpose = u.docPurpose,
                    docName = u.docName,
                    isActive = u.isActive,
                    isMandatory = u.isMandatory,
                    isAlphanumeric = u.isAlphanumeric,
                    isExpiry = u.isExpiry,
                    companyID = u.companyID,
                    ExpiryDate = u.ExpiryDate
                })
                .ToListAsync();

            return new PagedResultDto<XRS_Documents>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<XRS_Documents?> GetDocumentById(int documentId)
        {
            return await _context.Documents
                .Where(u => u.docTypeId == documentId)
                .Select(u => new XRS_Documents
                {
                    docTypeId = u.docTypeId,
                    docPurpose = u.docPurpose,
                    docName = u.docName,
                    isActive = u.isActive,
                    isMandatory = u.isMandatory,
                    isAlphanumeric = u.isAlphanumeric,
                    isExpiry = u.isExpiry,
                    ExpiryDate = u.ExpiryDate,
                    companyID = u.companyID 
                })
                .FirstOrDefaultAsync();
        }


        public async Task<XRS_Documents> CreateDocuments(XRS_Documents dtoDocuments)
        {

            var documents = new Models.XRS_Documents
            {
                docName = dtoDocuments.docName,
                docPurpose = dtoDocuments.docPurpose,
                companyID = dtoDocuments.companyID,         
                isActive = dtoDocuments.isActive,
                isMandatory = dtoDocuments.isMandatory,
                isAlphanumeric = dtoDocuments.isAlphanumeric,
                isExpiry = dtoDocuments.isExpiry,
                ExpiryDate= dtoDocuments.ExpiryDate


            };
           


            await _context.Documents.AddAsync(documents);
            await _context.SaveChangesAsync();
            return documents;

        }

        public async Task<bool> DeleteDocumentType(int id)
        {
            var documents = await _context.Documents.FirstOrDefaultAsync(u => u.companyID == id);
            if (documents == null) return false;
            documents.isActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateDocuments(int id, XRS_Documents documents)
        {
            var updatedDocuments = await _context.Documents.FirstOrDefaultAsync(u => u.docTypeId == id);
            if (updatedDocuments == null) return false;

            updatedDocuments.docName = documents.docName ?? documents.docName;
            updatedDocuments.companyID = documents.companyID;
            updatedDocuments.docPurpose = documents.docPurpose;
            updatedDocuments.isExpiry = documents.isExpiry;
            updatedDocuments.isAlphanumeric = documents.isAlphanumeric;
            updatedDocuments.isMandatory = documents.isMandatory;
            updatedDocuments.isActive = documents.isActive;
            updatedDocuments.ExpiryDate = documents.ExpiryDate;
            await _context.SaveChangesAsync();
            return true;
        }
     
    }
}
