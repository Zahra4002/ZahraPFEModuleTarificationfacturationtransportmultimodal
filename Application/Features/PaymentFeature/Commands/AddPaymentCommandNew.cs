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
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.PaymentFeature.Commands
{
    public record AddPaymentCommandNew(
        Guid? invoiceId,
        DateTime paymentDate,
        decimal amount,
        PaymentMethod paymentMethod,
        string reference,
        string notes
        ) : IRequest<ResponseHttp>
    {
        public class AddPaymentCommandNewHandler : IRequestHandler<AddPaymentCommandNew, ResponseHttp>
        {
            private readonly IPaymentRepository paymentRepository;
            private readonly IMapper _mapper;

            public AddPaymentCommandNewHandler(IPaymentRepository paymentRepository, IMapper mapper)
            {
                this.paymentRepository = paymentRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(AddPaymentCommandNew request, CancellationToken cancellationToken)
            {
                try
                {
                    var payment = _mapper.Map<Payment>(request);

                    payment = await paymentRepository.Post(payment);
                    await paymentRepository.SaveChange(cancellationToken);

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