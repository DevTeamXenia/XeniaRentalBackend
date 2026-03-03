
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Voucher
{
    public interface IVoucherRepository
    {
        Task<XRS_Voucher> CreateVoucherAsync(VoucherDto dto);
        Task<object?> GetVoucherByIdAsync(int id);
        Task<IEnumerable<object>> GetAllExpenseVouchersAsync(int companyId, DateTime? fromDate, DateTime? toDate, int? propertyId, string? voucherStatus, string? search);
        Task<IEnumerable<object>> GetAllVouchersAsync(int companyId, DateTime? fromDate, DateTime? toDate, int? propertyId, int? unitId, string? voucherStatus, string? search);
        Task<XRS_Voucher?> UpdateVoucherAsync(int id, VoucherDto dto);
        Task<XRS_Voucher> UpdatePaymentVoucherAsync(int id, VoucherDto dto);
        Task<XRS_Voucher> CreateIntiateAsync(VoucherCreateRequest request);
        Task<XRS_Voucher> UpdateAsync(int voucherId, VoucherCreateRequest request);
        Task<object> GetTenantChargesByMonthAsync(int companyId, int month, int year,int? propertyId = null, int? unitId = null, int? bedSpaceId = null, string? search = null);


    }
}
