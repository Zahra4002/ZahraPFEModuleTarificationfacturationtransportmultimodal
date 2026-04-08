using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.MerchandiseTypeFeature.Commands
{
    public record CreateMerchandiseTypeCommand(
        string Code,
        string Name,
        string? Description,
        int HazardousLevel,
        decimal? PriceMultiplier,
        bool RequiresSpecialHandling,
        bool IsActive
    ) : IRequest<ResponseHttp>;

    public class CreateMerchandiseTypeCommandHandler : IRequestHandler<CreateMerchandiseTypeCommand, ResponseHttp>
    {
        private readonly IMerchandiseTypeRepository _repository;

        public CreateMerchandiseTypeCommandHandler(IMerchandiseTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseHttp> Handle(CreateMerchandiseTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Vérifier si le code existe déjà
                var existing = await _repository.GetByCodeAsync(request.Code, cancellationToken);
                if (existing != null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Un type de marchandise avec le code '{request.Code}' existe déjà."
                    };
                }

                var entity = new MerchandiseType
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code.ToUpper(),
                    Name = request.Name,
                    Description = request.Description,
                    HazardousLevel = request.HazardousLevel,
                    PriceMultiplier = request.PriceMultiplier ?? 1.0m,
                    RequiresSpecialHandling = request.RequiresSpecialHandling,
                    IsActive = request.IsActive
                };

                // Utilisation de Post (méthode de IGenericRepository)
                await _repository.Post(entity);
                await _repository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status201Created,
                    Resultat = new { Id = entity.Id, Message = "Type de marchandise créé avec succès" }
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