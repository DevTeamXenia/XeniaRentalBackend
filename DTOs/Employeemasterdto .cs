using System.ComponentModel.DataAnnotations;

namespace XeniaRentalBackend.Dtos
{
    public class EmployeeMasterDto
    {
        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string WhatAppNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    //// For toggle switch only
    //public class UpdateEmployeeStatusDto
    //{
    //    [Required]
    //    public bool IsActive { get; set; }
    //}
}