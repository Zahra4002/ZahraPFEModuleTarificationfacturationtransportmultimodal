using Application.Features.InvoiceFeature.Dtos; // InvoiceDTO à créer
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.InvoiceFeature.Queries
{
    public record GetInvoiceByIdNewQuery(
        Guid InvoiceId
    ) : IRequest<ResponseHttp>
    {
        public class GetInvoiceByIdNewQueryHandler : IRequestHandler<GetInvoiceByIdNewQuery, ResponseHttp>
        {
            private readonly IInvoiceRepository _invoiceRepository;
            private readonly IMapper _mapper;

            public GetInvoiceByIdNewQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
            {
                _invoiceRepository = invoiceRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(GetInvoiceByIdNewQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    // Récupérer la facture par Id via repository
                    var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);

                    if (invoice == null)
                        return new ResponseHttp()
                        {
                            Status = StatusCodes.Status404NotFound,
                            Fail_Messages = "Invoice not found!"
                        };

                    // Mapper vers DTO
                    var invoiceDto = _mapper.Map<InvoiceDTO>(invoice);

                    return new ResponseHttp()
                    {
                        Resultat = invoiceDto,
                        Status = StatusCodes.Status200OK
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