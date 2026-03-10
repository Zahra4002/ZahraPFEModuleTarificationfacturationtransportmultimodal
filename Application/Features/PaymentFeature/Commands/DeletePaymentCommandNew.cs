using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.PaymentFeature.Commands
{
    public record DeletePaymentCommandNew(
        Guid InvoiceId
        )
        : IRequest<ResponseHttp>
    {

        public class DeletePaymentCommandNewHandler : IRequestHandler<DeletePaymentCommandNew, ResponseHttp>
        {
            private readonly IPaymentRepository paymentRepository;
            public DeletePaymentCommandNewHandler(IPaymentRepository paymentRepository)
            {
                this.paymentRepository = paymentRepository;
            }

            public async Task<ResponseHttp> Handle(DeletePaymentCommandNew request, CancellationToken cancellationToken)
            {
                var payment = await paymentRepository.GetById(request.InvoiceId);

                if (payment == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "No payment found",
                        Status = StatusCodes.Status400BadRequest,
                    };
                }

                await paymentRepository.SoftDelete(request.InvoiceId);
                await paymentRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                };
            }
        }
    }
}
