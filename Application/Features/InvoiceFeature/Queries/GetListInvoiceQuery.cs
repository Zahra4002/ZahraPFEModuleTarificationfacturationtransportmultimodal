using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.InvoiceFeature.Queries
{
    public class GetListInvoiceQuery : IRequest<ResponseHttp>
    {
        public class GetListInvoiceQueryHandler : IRequestHandler<GetListInvoiceQuery, ResponseHttp>
        {
            private readonly ICleanArchitecturContext _context;

            public GetListInvoiceQueryHandler(ICleanArchitecturContext context)
            {
                _context = context;
            }

            public async Task<ResponseHttp> Handle(GetListInvoiceQuery request, CancellationToken cancellationToken)
            {
                // Récupérer toutes les invoices non supprimées (si tu as une propriété IsDeleted)
                var invoices = await _context.Invoices
                    .Where(x => !x.IsDeleted) // ou x.IsDeleted == false
                    .ToListAsync(cancellationToken);

                return new ResponseHttp
                {
                    Status = 200,
                    Fail_Messages = "None",
                    Resultat = new
                    {
                        Invoices = invoices
                    }
                };
            }
        }
    }
}