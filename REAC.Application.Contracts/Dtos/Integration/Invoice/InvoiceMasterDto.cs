using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.Invoice
{
    public class InvoiceMasterDto
    {
        public int SourceID { get; set; }
        public string InvDueDate { get; set; }
        public int ClientNumber { get; set; }
        public int ProjectID { get; set; }
        public string Description { get; set; }
    }

    public class InvoiceTransactionDto
    {
        public int ServiceID { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateInvoiceRequest
    {
        public InvoiceMasterDto InvoiceMasterDto { get; set; }
        public List<InvoiceTransactionDto> InvoiceTransactionDto { get; set; }
        public int TypeOfInvoice { get; set; }
        public string ClientReferenceId { get; set; }
        public string ArName { get; set; }
        public string Type { get; set; }
        public string IdentityNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string CaseNumber { get; set; }
        public string CaseDescription { get; set; }
        public string RefIdPltfrm { get; set; }
    }

    public class CreateInvoiceResponseDto
    {
        public ApiMessageDto ApiMessage { get; set; }
        public InvoiceDataDto Data { get; set; }
    }

    public class ApiMessageDto
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public class InvoiceDataDto
    {
        public string Id { get; set; }
        public string InvDateJ { get; set; }
        public string SadadNumber { get; set; }
        public string SadadStatus { get; set; }
        public string InvoiceType { get; set; }
        public string InvDateH { get; set; }
        public string InvTime { get; set; }
        public string Description { get; set; }
        public string IsDeleted { get; set; }
        public string DeletedReason { get; set; }
        public string Reference { get; set; }
        public string Source { get; set; }
        public string Project { get; set; }
        public string Client { get; set; }
        public string CreatedBy { get; set; }
        public string InvDueDate { get; set; }
        public string InvoicedNumber { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string NeighborhoodName { get; set; }
        public string Price { get; set; }
        public string TotalQuantity { get; set; }
        public string TotalPrice { get; set; }
        public string TotalNonTaxable { get; set; }
        public string TotalTaxable { get; set; }
        public string TotalTax { get; set; }
        public string TotalTotalAfterTax { get; set; }
    }

}
