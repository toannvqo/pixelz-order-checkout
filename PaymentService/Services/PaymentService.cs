using Grpc.Core;
using System.Threading.Tasks;
namespace PaymentServiceServices
{
    public class PaymentService : PaymentServiceServices.Payment.PaymentBase
    {
        public override async Task<PaymentResponse> ProcessPayment(PaymentRequest request, ServerCallContext context)
        {
            // Logic to process payment
            var success = request.Amount <= 500; // Payment is successful if amount is less than or equal to 500

            return new PaymentResponse { Success = success };
        }
    }
}