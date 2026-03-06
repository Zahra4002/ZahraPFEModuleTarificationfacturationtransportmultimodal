using Application.Features.InvoiceFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.InvoiceFeature.Queries
{
    public class GetOverdueInvoicesQueryHandler : IRequestHandler<GetOverdueInvoicesQuery, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public GetOverdueInvoicesQueryHandler(
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetOverdueInvoicesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Utiliser la méthode du repository (plus efficace)
                var overdueInvoices = await _invoiceRepository.GetOverdueInvoicesAsync();

                // Mapper vers DTO
                var overdueDtos = _mapper.Map<List<OverdueInvoiceDto>>(overdueInvoices);

                // Calculer les jours de retard
                foreach (var dto in overdueDtos)
                {
                    dto.DaysOverdue = (DateTime.UtcNow - dto.DueDate).Days;
                }

                return new ResponseHttp
                {
                    Resultat = overdueDtos,
                    Status = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                };
            }
        }
    }
}