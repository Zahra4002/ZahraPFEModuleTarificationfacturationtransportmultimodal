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
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.PaymentFeature.Queries
{
    public record GetAllPaymentNewQuery(int? pageNumber, int? pageSize) : IRequest<ResponseHttp>
    {
        public class GetAllPaymentNewQueryHandler : IRequestHandler<GetAllPaymentNewQuery, ResponseHttp>
        {
            private readonly IPaymentRepository paymentRepository;
            private readonly IMapper _mapper;

            public GetAllPaymentNewQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
            {
                this.paymentRepository = paymentRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(GetAllPaymentNewQuery request, CancellationToken cancellationToken)
            {
                var payment = await paymentRepository.GetAllWithTypesAsync(request.pageNumber, request.pageSize, cancellationToken);

                if (payment == null)
                    return new ResponseHttp
                    {
                        Fail_Messages = "No payment found !",
                        Status = StatusCodes.Status400BadRequest,
                    };

                var customersToReturn = _mapper.Map<PagedList<PaymentDTO>>(payment);
                return new ResponseHttp
                {
                    Status = 200,
                    Resultat = customersToReturn
                };
            }
        }
    }
}
