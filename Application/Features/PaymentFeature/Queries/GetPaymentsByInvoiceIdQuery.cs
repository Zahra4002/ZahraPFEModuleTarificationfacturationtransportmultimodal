using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.PaymentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;

namespace Application.Features.PaymentFeature.Queries
{
    public record GetPaymentsByInvoiceIdQuery(string InvoiceId) : IRequest<ResponseHttp>;

    public class GetPaymentsByInvoiceIdQueryHandler : IRequestHandler<GetPaymentsByInvoiceIdQuery, ResponseHttp>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public GetPaymentsByInvoiceIdQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetPaymentsByInvoiceIdQuery request, CancellationToken ct)
        {
            var payments = await _paymentRepository.GetByInvoiceAsync(request.InvoiceId, ct);

            if (!payments.Any())
                return new ResponseHttp { Status = 404, Fail_Messages = "Aucun paiement trouvé pour cette facture" };

            var dtos = _mapper.Map<List<PaymentDTO>>(payments);

            return new ResponseHttp { Status = 200, Resultat = dtos };
        }
    }
}
