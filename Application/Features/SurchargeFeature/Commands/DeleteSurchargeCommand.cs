using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SurchargeFeature.Commands
{
    public record DeleteSurchargeCommand(Guid Id) : IRequest<ResponseHttp>;

    public class DeleteSurchargeCommandHandler : IRequestHandler<DeleteSurchargeCommand, ResponseHttp>
    {
        private readonly ISurchargeRepository _surchargeRepository;

        public DeleteSurchargeCommandHandler(ISurchargeRepository surchargeRepository)
        {
            _surchargeRepository = surchargeRepository;
        }

        public async Task<ResponseHttp> Handle(DeleteSurchargeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var surcharge = await _surchargeRepository.GetByIdAsync(request.Id, cancellationToken);
                if (surcharge == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Surcharge not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Soft delete all rules first
                var rules = await _surchargeRepository.GetRulesBySurchargeIdAsync(request.Id, cancellationToken);
                foreach (var rule in rules)
                {
                    await _surchargeRepository.DeleteRuleAsync(rule.Id);
                }

                await _surchargeRepository.SoftDelete(request.Id);
                await _surchargeRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = true
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