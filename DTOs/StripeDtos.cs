using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AddCustomerResponseDto
    {
        public string CustomerId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class StripePaymentResponse
    {
        public string CustomerId { get; set; } = null!;
        public string ReceiptEmail { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public long Amount { get; set; }
        public string PaymentId { get; set; } = null!;
    }
}
