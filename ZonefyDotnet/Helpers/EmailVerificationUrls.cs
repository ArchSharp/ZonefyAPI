using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Configurations
{
    public class EmailVerificationUrls
    {
        public string Verify { get; set; } = null!;
        public string Reset { get; set; } = null!;
    }
}
