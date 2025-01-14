using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REAC.Application.Contracts.Dtos.Integration.GEE
{
    public class SMSRequestDto
    {
        public string message_text { get; set; }
        public string notification_type { get; set; }
        public List<string> recipients { get; set; }
    }

    public class SMSResponseDto
    {
        public SMSDataDto? Data { get; set; }
        public SMSErrorDto? SMSError { get; set; }
        public GeneralGEEErrorDto? SMSUnauthorized { get; set; }
    }

    public class SMSDataDto
    {
        public string status { get; set; }
        public ApiResponseDto api_response { get; set; }
    }
    public class ApiResponseDto
    {
        public int replyCode { get; set; }
        public string replyMessage { get; set; }
        public string requestId { get; set; }
        public int clientRequestId { get; set; }
        public string requestTime { get; set; }
        public DataDto data { get; set; }
    }

    public class DataDto
    {
        public string InvalidRecipients { get; set; }
        public List<ErrorsDto>? Errors { get; set; }
        public List<SentSMSIDDto> SentSMSIDs { get; set; }
    }

    public class SentSMSIDDto
    {
        public string SMSId { get; set; }
    }

    public class ErrorsDto
    {

    }

    public class SMSErrorDto
    {
        public List<DetailDto> detail { get; set; }
    }

    public class DetailDto
    {
        public string type { get; set; }
        public string msg { get; set; }
        public string input { get; set; }
        public object[] loc { get; set; }
        public CtxDto ctx { get; set; }
    }

    public class CtxDto
    {
        public string expected { get; set; }
    }

}
