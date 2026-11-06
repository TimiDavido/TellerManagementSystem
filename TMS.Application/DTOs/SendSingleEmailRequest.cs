using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Application.DTOs
{
    public class SendSingleEmailRequest
    {
        public string? Sender { get; set; }
        public string? Receiver { get; set; }
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public string? Body { get; set; }
        public string? Subject { get; set; }
    }
}
