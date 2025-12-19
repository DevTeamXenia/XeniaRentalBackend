using XeniaRentalBackend.Dtos;

namespace XeniaRentalBackend.Repositories.Charges
{
    public interface IChargesRepository
    {
        Task<IEnumerable<ChargesDto>> GetCharges(int companyId, int? propertyId = null);
        Task<PagedResultDto<ChargesDto>> GetChargesByCompanyId(int companyId, int? propertyId = null, string? search = null,int pageNumber = 1, int pageSize = 10);
        Task<ChargesDto?> GetChargesById(int chargeId);

        Task<Models.XRS_Charges> CreateCharges(ChargesDto charges);

        Task<bool> UpdateCharges(int id, Models.XRS_Charges charges);
        Task<bool> DeleteCharges(int id);
    }
}
