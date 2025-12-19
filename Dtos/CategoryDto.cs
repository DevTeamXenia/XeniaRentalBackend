using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Dtos
{
    public class CategoryDto
    {
        public int CompanyID { get; set; }

        public string CategoryName { get; set; }

        public bool IsActive { get; set; }

    }
}
