using System.Collections.Generic;

namespace Domain.Entities
{
    public class EmailRequest
    {
        public string EmailSubject { get; set; }
        public string ReceiverEmail { get; set; }
        public string TemplateName { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }
}