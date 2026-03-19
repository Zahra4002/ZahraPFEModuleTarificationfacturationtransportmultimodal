using Application.Features.SurchargeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Application.Features.SurchargeFeature.Commands
{
    public record UpdateSurchargeRuleCommand(Guid SurchargeId, Guid RuleId, UpdateSurchargeRuleDTO RuleDto) : IRequest<ResponseHttp>;

    public class UpdateSurchargeRuleCommandHandler : IRequestHandler<UpdateSurchargeRuleCommand, ResponseHttp>
    {
        private readonly ISurchargeRepository _surchargeRepository;
        private readonly IMapper _mapper;

        public UpdateSurchargeRuleCommandHandler(ISurchargeRepository surchargeRepository, IMapper mapper)
        {
            _surchargeRepository = surchargeRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateSurchargeRuleCommand request, CancellationToken cancellationToken)
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

                // Get the rule
                var rule = await _surchargeRepository.GetRuleByIdAsync(request.RuleId, cancellationToken);
                if (rule == null || rule.SurchargeId != request.SurchargeId)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Rule not found in this surcharge",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Check for duplicate rule name if name changed
                if (rule.Name != request.RuleDto.Name)
                {
                    var exists = await _surchargeRepository.RuleExistsInSurchargeAsync(
                        request.SurchargeId,
                        request.RuleDto.Name,
                        request.RuleId);

                    if (exists)
                    {
                        return new ResponseHttp
                        {
                            Fail_Messages = $"A rule with name '{request.RuleDto.Name}' already exists in this surcharge",
                            Status = StatusCodes.Status400BadRequest
                        };
                    }
                }

                // Serialize transport modes to JSON
                string? transportModesJson = null;
                if (request.RuleDto.ApplicableTransportModes != null && request.RuleDto.ApplicableTransportModes.Any())
                {
                    transportModesJson = JsonSerializer.Serialize(request.RuleDto.ApplicableTransportModes);
                }

                rule.Name = request.RuleDto.Name;
                rule.ConditionsJson = request.RuleDto.ConditionsJson ?? "{}";
                rule.ValidFrom = request.RuleDto.ValidFrom;
                rule.ValidTo = request.RuleDto.ValidTo;
                rule.ZoneFromId = request.RuleDto.ZoneFromId;
                rule.ZoneToId = request.RuleDto.ZoneToId;
                rule.ApplicableTransportModes = transportModesJson;
                rule.OverrideValue = request.RuleDto.OverrideValue;
                rule.Priority = request.RuleDto.Priority;
                rule.ModifiedDate = DateTime.UtcNow;

                await _surchargeRepository.Update(surcharge);
                await _surchargeRepository.SaveChange(cancellationToken);

                // Get updated rule with details
                var updatedRule = await _surchargeRepository.GetRuleByIdWithDetailsAsync(rule.Id, cancellationToken);
                var result = _mapper.Map<SurchargeRuleDTO>(updatedRule);

                return new ResponseHttp
                {
                    Resultat = result,
                    Status = StatusCodes.Status200OK
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