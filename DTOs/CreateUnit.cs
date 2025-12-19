using System.ComponentModel.DataAnnotations;

namespace XeniaRentalBackend.DTOs
{
    public class CreateUnit
    {
        public int CompanyId { get; set; }

        /// <summary>
        /// PropertyId
        /// Required
        /// </summary>
        public int PropID { get; set; }

        public int? CatID { get; set; }

        /// <summary>
        /// UnitName
        /// Required
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string UnitName { get; set; }

       
        /// <summary>
        /// Unit Type
        /// optional
        /// </summary>
        public string UnitType { get; set; }

        /// <summary>
        /// Is Active
        /// Required
        /// </summary>
        public bool IsActive { get; set; }

        public string FloorNo { get; set; }

        public string Area { get; set; }


        public string Remarks { get; set; }

        public decimal DefaultRent { get; set; }

        public decimal escalationper { get; set; }
    }
}
