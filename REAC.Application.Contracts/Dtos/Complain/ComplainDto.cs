using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Complain
{
    public class ComplainDto
    {
        public long Id { get; set; }
        public string TicketNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeSpan CreatedTime { get; set; }
        public string TicketDescription { get; set; }
        public Guid ApplicantCitizenId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNID { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string ResponseText { get; set; }
        public DateTime? ResponseDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int ComplainsStatusId { get; set; }
        public string StatusName { get; set; }
        public int ComplainsTypeId { get; set; }
        public string TypeName { get; set; }
        public List<ComplainFileDto> Attachments { get; set; } = new List<ComplainFileDto>();
    }

    public class ComplainFileDto
    {
        public Guid Id { get; set; }
        public Guid FileManagerId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }

    public class CreateComplainDto
    {
        [Required(ErrorMessage = "وصف الشكوى/الاستفسار مطلوب")]
        [StringLength(4000, ErrorMessage = "وصف الشكوى يجب أن يكون أقل من 4000 حرف")]
        public string TicketDescription { get; set; }

        [Required(ErrorMessage = "نوع الطلب مطلوب")]
        public int ComplainsTypeId { get; set; }

        [Required(ErrorMessage = "اسم مقدم الطلب مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم يجب أن يكون أقل من 100 حرف")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "رقم الهوية مطلوب")]
        [StringLength(20, ErrorMessage = "رقم الهوية يجب أن يكون أقل من 20 حرف")]
        public string CustomerNID { get; set; }

        [Required(ErrorMessage = "رقم الجوال مطلوب")]
        [StringLength(15, ErrorMessage = "رقم الجوال يجب أن يكون أقل من 15 رقم")]
        [Phone(ErrorMessage = "رقم الجوال غير صحيح")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [StringLength(100, ErrorMessage = "البريد الإلكتروني يجب أن يكون أقل من 100 حرف")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public string Email { get; set; }

        public List<Guid>? Attachments { get; set; }
    }

    public class ComplainSearchDto
    {
        public string? TicketNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ComplainsTypeId { get; set; }
        public List<int> StatusIds { get; set; } = new List<int>();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsStaff { get; set; } 
    }

    public class UpdateComplainResponseDto
    {
        [Required(ErrorMessage = "نص الرد مطلوب")]
        public string ResponseText { get; set; }
    }

    public class ComplainReportDto
    {
        public int? ComplainsTypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<int> StatusIds { get; set; } = new List<int>();
        public bool OverdueOnly { get; set; }
    }

    public class BaseResponse<T>
    {
        public bool IsDone { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }


}
