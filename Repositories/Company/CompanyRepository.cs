using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;


namespace XeniaRentalBackend.Repositories.Company
{
    public class CompanyRepository:ICompanyRepsitory
    {

        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<XRS_Company>> GetCompanies(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var companies = await _context.Company
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return companies;
        }

        public async Task<IEnumerable<XRS_Company>> GetCompanybyId(int companyId)
        {

            return await _context.Company
                .Where(u => u.companyID == companyId)
                 .ToListAsync();

        }



        public async Task<XRS_Company> CreateCompany(XRS_Company company)
        {
  
            await _context.Company.AddAsync(company);
            await _context.SaveChangesAsync();

         
            var adminUser = new XRS_Users
            {
                CompanyId = company.companyID,
                UserType = 2, 
                UserName = "admin",
                Password = "admin", 
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Users.AddAsync(adminUser);
            await _context.SaveChangesAsync();

            return company;
        }



        public async Task<bool> DeleteCompany(int id)
        {
            var bedspacesettings = await _context.Company.FirstOrDefaultAsync(u => u.companyID == id);
            if (bedspacesettings == null) return false;
            bedspacesettings.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> UpdateCompany(int id, XRS_Company company)
        {
            var updatedCompany = await _context.Company.FirstOrDefaultAsync(u => u.companyID == id);
            if (updatedCompany == null) return false;

            updatedCompany.companyName = company.companyName ?? company.companyName;
            updatedCompany.companyID = company.companyID;
            updatedCompany.phoneNumber = company.phoneNumber;
            updatedCompany.address = company.address;
            updatedCompany.pin = company.pin;
            updatedCompany.email = company.email;
            updatedCompany.logo = company.logo;
            updatedCompany.IsActive = company.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }
     
    }
}
