using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.PaymentFeature.Dtos;
using Application.Features.TestFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.PaymentFeature.Queries
{
    public record GetPaymByIdNewQuery(
         Guid InvoiceId
         ) : IRequest<ResponseHttp>
    {
        public class GetPaymByIdNewQueryHandler : IRequestHandler<GetPaymByIdNewQuery, ResponseHttp>
        {
            private readonly IPaymentRepository paymentRepository;
            private readonly IMapper _mapper;

            public GetPaymByIdNewQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
            {
                this.paymentRepository = paymentRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(GetPaymByIdNewQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var payment = await paymentRepository.GetByIdAsync(request.InvoiceId, cancellationToken);

                    if (payment == null)
                        return new ResponseHttp()
                        {
                            Status = 404,
                            Fail_Messages = "payment not found !"
                        };

                    return new ResponseHttp()
                    {

                        Resultat = _mapper.Map<PaymentDTO>(payment),
                        Status = 200
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = ex.Message,
                        Status = StatusCodes.Status400BadRequest,
                    };

                }

            }
        }
    }
}
