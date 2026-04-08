using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.MerchandiseTypeFeature.Commands
{
    public record DeleteMerchandiseTypeCommand(Guid Id) : IRequest<ResponseHttp>;

    public class DeleteMerchandiseTypeCommandHandler : IRequestHandler<DeleteMerchandiseTypeCommand, ResponseHttp>
    {
        private readonly IMerchandiseTypeRepository _repository;

        public DeleteMerchandiseTypeCommandHandler(IMerchandiseTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseHttp> Handle(DeleteMerchandiseTypeCommand request, CancellationToken cancellationToken)
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

                await _repository.SoftDelete(request.Id);
                await _repository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = "Type de marchandise supprimé avec succès"
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