using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Documents;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;


        public DocumentsController(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<XRS_Documents>>> Get(int companyId)
        {
            var documents = await _documentRepository.GetDocuments(companyId);
            if (documents == null || !documents.Any())
            {
                return NotFound(new { Status = "Error", Message = "No documents found." });
            }
            return Ok(new { Status = "Success", Data = documents });
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<PagedResultDto<XRS_Documents>>> GetDocumentsByCompanyId(int companyId, string? search = null, string? docPurpose = null, int pageNumber = 1, int pageSize = 10)
        {

            var documents = await _documentRepository.GetDocumentsCompanyId(companyId, search, docPurpose, pageNumber, pageSize);
            if (documents == null)
            {
                return NotFound(new { Status = "Error", Message = "No documents found the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = documents });
        }


        [HttpPost]
        public async Task<IActionResult> CreateAccounts([FromBody] XRS_Documents documents)
        {
            if (documents == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid documents group." });
            }

            var createdDocuments = await _documentRepository.CreateDocuments(documents);
            return CreatedAtAction(nameof(GetDocuments), new { id = createdDocuments }, new { Status = "Success", Data = createdDocuments });
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_Documents>> GetDocuments(int id)
        {
            var createdDocuments = await _documentRepository.GetDocumentById(id);
            if (createdDocuments == null)
            {
                return NotFound(new { Status = "Error", Message = "Documents not found." });
            }
            return Ok(new { Status = "Success", Data = createdDocuments });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocuments(int id, [FromBody] Models.XRS_Documents documents)
        {
            if (documents == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid documents data" });
            }

            var updated = await _documentRepository.UpdateDocuments(id, documents);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "documents not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "Documents updated successfully." });
        }

  
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var deleted = await _documentRepository.DeleteDocumentType(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "Documents not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "createdDocuments deleted successfully." });
        }

      
    }
}
