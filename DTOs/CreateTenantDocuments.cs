using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.DTOs
{
    public class CreateTenantDocuments
    {
        
        
        public int tenantDocId { get; set; }


        public int DocTypeId { get; set; }

        /// <summary>
        /// ComapnyId
        /// required
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TenantID { get; set; }

        /// <summary>
        /// DocumentsNo
        /// optional
        /// </summary>
        public string DocumentsNo { get; set; }

        /// <summary>
        /// documents url
        /// optional
        /// </summary>
        public string Documenturl { get; set; }

        public bool isActive { get; set; }

        [ForeignKey("DocTypeId")]
        [JsonIgnore]
        public virtual XRS_Documents? Documents { get; set; }
    }
}
