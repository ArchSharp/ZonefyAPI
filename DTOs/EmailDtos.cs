using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonefyDotnet.DTOs
{
    public class EmailDtos
    {
        public string To { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;

    }

    public class EmailSender
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
}
