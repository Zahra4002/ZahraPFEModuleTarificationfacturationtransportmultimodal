using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SupplierFeature.Commands
{
    public record DeleteSupplierCommand(Guid Id) : IRequest<ResponseHttp>;
    public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, ResponseHttp>
    {
        private readonly ISupplierRepository _supplierRepository;

        public DeleteSupplierCommandHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<ResponseHttp> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
                if (supplier == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Fournisseur avec ID {request.Id} non trouvé"
                    };
                }

                // Vérifier si le fournisseur est utilisé (optionnel)
                // if (await _supplierRepository.HasContractsAsync(request.Id))
                // {
                //     return new ResponseHttp
                //     {
                //         Status = StatusCodes.Status400BadRequest,
                //         Fail_Messages = "Impossible de supprimer un fournisseur avec des contrats actifs"
                //     };
                // }

                // Soft delete
                supplier.IsDeleted = true;
                supplier.DeletedDate = DateTime.UtcNow;

                await _supplierRepository.UpdateAsync(supplier, cancellationToken);
                await _supplierRepository.SaveChangesAsync(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status204NoContent
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