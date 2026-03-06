using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.InvoiceFeature.Commands
{
    public record DeleteInvoiceCommand(
        Guid InvoiceId
    ) : IRequest<ResponseHttp>
    {

        public class DeleteInvoiceCommandHandler
            : IRequestHandler<DeleteInvoiceCommand, ResponseHttp>
        {
            private readonly IInvoiceRepository _invoiceRepository;

            public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
            {
                _invoiceRepository = invoiceRepository;
            }

            public async Task<ResponseHttp> Handle(
                DeleteInvoiceCommand request,
                CancellationToken cancellationToken)
            {
                // 1️⃣ Vérifier si la facture existe
                var invoice = await _invoiceRepository.GetById(request.InvoiceId);

                if (invoice == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Aucune facture trouvée",
                        Status = StatusCodes.Status400BadRequest,
                    };
                }

                // 2️⃣ Soft Delete (IsDeleted = true)
                await _invoiceRepository.SoftDelete(request.InvoiceId);

                // 3️⃣ Sauvegarder en base
                await _invoiceRepository.SaveChange(cancellationToken);

                // 4️⃣ Retourner succès
                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                };
            }
        }
    }
}