using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Voucher
{
    public interface IVoucherRepository
    {
        Task<XRS_Voucher> CreateVoucherAsync(VoucherDto dto);
        Task<object?> GetVoucherByIdAsync(int id);
        Task<IEnumerable<object>> GetAllVouchersAsync(int companyId, string? search = null);
        Task<XRS_Voucher?> UpdateVoucherAsync(int id, VoucherDto dto);
        Task<bool> DeleteVoucherAsync(int id);
        Task<XRS_Voucher> CreateIntiateAsync(VoucherCreateRequest request);
        Task<object> GetTenantChargesByMonthAsync(int month, int year);


    }
}
