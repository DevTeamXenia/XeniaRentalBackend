using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XeniaTenoraBackend.Models;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Employee")]
    public class XRS_Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string WhatAppNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ICollection<XRS_EmployeeArea> EmployeeAreas { get; set; }

        = new List<XRS_EmployeeArea>();
        public string? CategoryName { get; set; }
    }
}