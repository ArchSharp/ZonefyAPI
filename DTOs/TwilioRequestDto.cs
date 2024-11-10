using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TwilioRequestDto
    {
        [MaxLength(200)]
        [Required]
        public string Message { get; set; }
        [Required]
        public string To { get; set; }
    }
}
