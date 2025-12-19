using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Category
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<XRS_Categories>> GetCategories();

        Task<PagedResultDto<XRS_Categories>> GetCategorybyCompanyId(int companyId, string? search = null,int pageNumber = 1, int pageSize = 10);

        Task<IEnumerable<XRS_Categories>> GetCategorybyId(int Id);

        Task<XRS_Categories> CreateCategory(CategoryDto category);

        Task<bool> UpdateCategory(int id, CategoryDto bedSpace);

        Task<bool> DeleteCategory(int id);

    }
}
