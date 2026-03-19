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
    public record UpdatePaymentCommandNew(
        Guid PaymentId,
        DateTime paymentDate,
        decimal amount,
        PaymentMethod paymentMethod,
        string reference,
        string notes)
       : IRequest<ResponseHttp>

    {
        public class UpdatePaymentCommandNewHandler : IRequestHandler<UpdatePaymentCommandNew, ResponseHttp>
        {
            private readonly IPaymentRepository paymentRepository;
            private readonly IMapper _mapper;

            public UpdatePaymentCommandNewHandler(IPaymentRepository paymentRepository, IMapper mapper)
            {
                this.paymentRepository = paymentRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(UpdatePaymentCommandNew request, CancellationToken cancellationToken)
            {
                Payment? payment = await paymentRepository.GetById(request.PaymentId);

                if (payment == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = this._mapper.Map<PaymentDTO>(payment),
                        Fail_Messages = "Customer with this Id not found.",
                        Status = StatusCodes.Status400BadRequest,
                    };
                }
                else
                {
                    payment.Id = request.PaymentId;
                    payment.PaymentDate = request.paymentDate;
                    payment.Amount = request.amount;
                    payment.PaymentMethod = request.paymentMethod;
                    payment.Reference = request.reference;
                    payment.Notes = request.notes;
                    await paymentRepository.Update(payment);
                    await paymentRepository.SaveChange(cancellationToken);

                    var customerToReturn = _mapper.Map<PaymentDTO>(payment);
                    return new ResponseHttp
                    {
                        Resultat = customerToReturn,
                        Status = StatusCodes.Status200OK,
                    };

                }

            }
        }
    }
}
