using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Application.DTOs
{
    public class SendSingleEmailResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;
    }
}
