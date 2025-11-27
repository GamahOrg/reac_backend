using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    [Table("LitigationRequestInvoice")]
    public class LitigationRequestInvoice : BaseEntity<Guid>
    {
        [Column("ReferenceId")]
        [StringLength(50)]
        public string? ReferenceId { get; set; }

        // Foreign Key to LitigationRequest
        [Required]
        [Column("litigationRequestId")]
        [StringLength(50)]
        public Guid? LitigationRequestId { get; set; }

        [Column("invDateJ")]
        public DateTime? InvDateJ { get; set; }

        [Column("sadadNumber")]
        [StringLength(50)]
        public string? SadadNumber { get; set; }

        [Column("sadadStatus")]
        [StringLength(100)]
        public string? SadadStatus { get; set; }

        [Column("invoiceType")]
        [StringLength(100)]
        public string? InvoiceType { get; set; }

        [Column("invDateH")]
        [StringLength(20)]
        public string? InvDateH { get; set; }

        [Column("invTime")]
        [StringLength(20)]
        public string? InvTime { get; set; }

        [Column("description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Column("isDeleted")]
        [StringLength(10)]
        public string? IsDeleted { get; set; }

        [Column("deletedReason")]
        [StringLength(500)]
        public string? DeletedReason { get; set; }

        [Column("reference")]
        [StringLength(100)]
        public string? Reference { get; set; }

        [Column("source")]
        [StringLength(200)]
        public string? Source { get; set; }

        [Column("project")]
        [StringLength(200)]
        public string? Project { get; set; }

        [Column("client")]
        [StringLength(200)]
        public string? Client { get; set; }

        [Column("createdBy")]
        [StringLength(200)]
        public string? CreatedBy { get; set; }

        [Column("invDueDate")]
        public DateTime? InvDueDate { get; set; }

        [Column("invoicedNumber")]
        [StringLength(50)]
        public string? InvoicedNumber { get; set; }

        [Column("provinceName")]
        [StringLength(100)]
        public string? ProvinceName { get; set; }

        [Column("cityName")]
        [StringLength(100)]
        public string? CityName { get; set; }

        [Column("neighborhoodName")]
        [StringLength(100)]
        public string? NeighborhoodName { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("totalQuantity")]
        public int? TotalQuantity { get; set; }

        [Column("totalPrice")]
        public decimal? TotalPrice { get; set; }

        [Column("totalNonTaxable")]
        public decimal? TotalNonTaxable { get; set; }

        [Column("totalTaxable")]
        public decimal? TotalTaxable { get; set; }

        [Column("totalTax")]
        public decimal? TotalTax { get; set; }

        [Column("totalTotalAfterTax")]
        public decimal? TotalTotalAfterTax { get; set; }

        // Additional properties as requested
        [Column("statusId")]
        public int? StatusId { get; set; }

        [Column("updatedOn")]
        public DateTime? UpdatedOn { get; set; }

        [Column("createdOn")]
        public DateTime? CreatedOn { get; set; }

        [Column("serviceId")]
        [StringLength(50)]
        public string? serviceId { get; set; }

        // Navigation property
        [ForeignKey("LitigationRequestId")]
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
    }
}
