// Application/Features/SurchargeFeature/Commands/DeleteSurchargeRuleCommand.cs
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SurchargeFeature.Commands
{
    public record DeleteSurchargeRuleCommand(Guid SurchargeId, Guid RuleId) : IRequest<ResponseHttp>;

    public class DeleteSurchargeRuleCommandHandler : IRequestHandler<DeleteSurchargeRuleCommand, ResponseHttp>
    {
        private readonly ISurchargeRepository _surchargeRepository;

        public DeleteSurchargeRuleCommandHandler(ISurchargeRepository surchargeRepository)
        {
            _surchargeRepository = surchargeRepository;
        }

        public async Task<ResponseHttp> Handle(DeleteSurchargeRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if surcharge exists
                var surcharge = await _surchargeRepository.GetByIdAsync(request.SurchargeId, cancellationToken);
                if (surcharge == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Surcharge not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Get the rule to verify it belongs to this surcharge
                var rule = await _surchargeRepository.GetRuleByIdAsync(request.RuleId, cancellationToken);
                if (rule == null || rule.SurchargeId != request.SurchargeId)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Rule not found in this surcharge",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var result = await _surchargeRepository.DeleteRuleAsync(request.RuleId);
                if (!result)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Failed to delete rule",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                await _surchargeRepository.SaveChange(cancellationToken);

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