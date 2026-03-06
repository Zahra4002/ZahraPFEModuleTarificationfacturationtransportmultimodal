using Application.Features.InvoiceFeature.Dtos; 
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.InvoiceFeature.Queries
{
    public record GetAllInvoiceQuery(int? pageNumber, int? pageSize) : IRequest<ResponseHttp>
    {
        public class GetAllInvoiceQueryHandler : IRequestHandler<GetAllInvoiceQuery, ResponseHttp>
        {
            private readonly IInvoiceRepository _invoiceRepository;
            private readonly IMapper _mapper;

            public GetAllInvoiceQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
            {
                _invoiceRepository = invoiceRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(GetAllInvoiceQuery request, CancellationToken cancellationToken)
            {
                // Récupérer les invoices avec pagination
                var invoices = await _invoiceRepository.GetAllWithDetailsAsync(request.pageNumber, request.pageSize, cancellationToken);

                if (invoices == null || !invoices.Items.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "No invoices found!",
                        Status = StatusCodes.Status400BadRequest,
                    };
                }

                // Mapper vers DTO
                var invoicesToReturn = _mapper.Map<PagedList<InvoiceDTO>>(invoices);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = invoicesToReturn
                };
            }
        }
    }
}
