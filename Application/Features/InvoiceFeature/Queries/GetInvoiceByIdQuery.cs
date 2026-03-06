using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.InvoiceFeature.Queries
{
    public class GetInvoiceByIdQuery : IRequest<ResponseHttp>
    {
        public Guid Id { get; set; }

        public GetInvoiceByIdQuery(Guid id)
        {
            Id = id;
        }

        public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, ResponseHttp>
        {
            private readonly ICleanArchitecturContext _context;

            public GetInvoiceByIdQueryHandler(ICleanArchitecturContext context)
            {
                _context = context;
            }

            public async Task<ResponseHttp> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
            {
                var invoice = await _context.Invoices
                    .Where(x => x.Id == request.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                if (invoice == null)
                    return new ResponseHttp()
                    {
                        Resultat = "Not Found",
                        Status = 404,
                        Fail_Messages = "No invoice exists with this Id"
                    };

                return new ResponseHttp()
                {
                    Resultat = invoice,
                    Status = 200,
                    Fail_Messages = "None"
                };
            }
        }
    }
}