using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.MerchandiseTypeFeature.Commands
{
    public record UpdateMerchandiseTypeCommand(
        Guid Id,
        string Code,
        string Name,
        string? Description,
        int HazardousLevel,
        decimal? PriceMultiplier,
        bool RequiresSpecialHandling,
        bool IsActive
    ) : IRequest<ResponseHttp>;

    public class UpdateMerchandiseTypeCommandHandler : IRequestHandler<UpdateMerchandiseTypeCommand, ResponseHttp>
    {
        private readonly IMerchandiseTypeRepository _repository;

        public UpdateMerchandiseTypeCommandHandler(IMerchandiseTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseHttp> Handle(UpdateMerchandiseTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (entity == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Type de marchandise non trouvé."
                    };
                }

                // Vérifier si le nouveau code existe déjà (pour un autre enregistrement)
                if (entity.Code != request.Code)
                {
                    var existing = await _repository.GetByCodeAsync(request.Code, cancellationToken);
                    if (existing != null && existing.Id != request.Id)
                    {
                        return new ResponseHttp
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Fail_Messages = $"Un type de marchandise avec le code '{request.Code}' existe déjà."
                        };
                    }
                }

                entity.Code = request.Code.ToUpper();
                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.HazardousLevel = request.HazardousLevel;
                entity.PriceMultiplier = request.PriceMultiplier ?? 1.0m;
                entity.RequiresSpecialHandling = request.RequiresSpecialHandling;
                entity.IsActive = request.IsActive;

                await _repository.Update(entity);
                await _repository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = "Type de marchandise mis à jour avec succès"
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status400BadRequest,
                    Fail_Messages = ex.Message
                };
            }
        }
    }
}