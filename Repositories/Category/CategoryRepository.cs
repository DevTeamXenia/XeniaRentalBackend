using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<XRS_Categories>> GetCategories()
        {

            return await _context.Category
                .Where(u => u.IsActive == true)
                 .Select(u => new Models.XRS_Categories
                 {
                     CategoryName = u.CategoryName,
                     CatID = u.CatID,
                     CompanyID = u.CompanyID,
                     IsActive = u.IsActive,

                 }).ToListAsync();


        }

        public async Task<PagedResultDto<XRS_Categories>> GetCategorybyCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10)
        {

            var query = _context.Category.AsQueryable();
            query = query.Where(u => u.CompanyID == companyId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.CategoryName.Contains(search));
            }
            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.CategoryName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                 .Select(u => new Models.XRS_Categories
                 {
                     CategoryName = u.CategoryName,
                     CatID = u.CatID,
                     CompanyID = u.CompanyID,
                     IsActive = u.IsActive,

                 }).ToListAsync();


            return new PagedResultDto<Models.XRS_Categories>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

        }

        public async Task<IEnumerable<XRS_Categories>> GetCategorybyId(int categoryId)
        {

            return await _context.Category
                .Where(u => u.CatID == categoryId)
                 .Select(u => new Models.XRS_Categories
                 {
                     CategoryName = u.CategoryName,
                     CatID = u.CatID,
                     CompanyID = u.CompanyID,
                     IsActive = u.IsActive,

                 }).ToListAsync();

        }

        public async Task<XRS_Categories> CreateCategory(CategoryDto dtoCategory)
        {

            var category = new Models.XRS_Categories
            {
                CategoryName = dtoCategory.CategoryName,
                IsActive = dtoCategory.IsActive,
                CompanyID = dtoCategory.CompanyID,

            };
            await _context.Category.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;

        }
   
        public async Task<bool> UpdateCategory(int id, CategoryDto category)
        {
            var updatebedSpace = await _context.Category.FirstOrDefaultAsync(u => u.CatID == id);
            if (updatebedSpace == null) return false;

            updatebedSpace.CategoryName = category.CategoryName;
            updatebedSpace.CompanyID = category.CompanyID;
            updatebedSpace.IsActive = category.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Category.FirstOrDefaultAsync(u => u.CatID == id);
            if (category == null) return false;
            category.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    
    }
}
