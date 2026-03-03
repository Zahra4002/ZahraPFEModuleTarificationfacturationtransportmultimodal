using Application.Features.SurchargeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Application.Features.SurchargeFeature.Commands
{
    public record CreateSurchargeRuleCommand(Guid SurchargeId, CreateSurchargeRuleDTO RuleDto) : IRequest<ResponseHttp>;

    public class CreateSurchargeRuleCommandHandler : IRequestHandler<CreateSurchargeRuleCommand, ResponseHttp>
    {
        private readonly ISurchargeRepository _surchargeRepository;
        private readonly IMapper _mapper;

        public CreateSurchargeRuleCommandHandler(ISurchargeRepository surchargeRepository, IMapper mapper)
        {
            _surchargeRepository = surchargeRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateSurchargeRuleCommand request, CancellationToken cancellationToken)
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

                // Check for duplicate rule name
                var exists = await _surchargeRepository.RuleExistsInSurchargeAsync(
                    request.SurchargeId,
                    request.RuleDto.Name);

                if (exists)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"A rule with name '{request.RuleDto.Name}' already exists in this surcharge",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                // Serialize transport modes to JSON
                string? transportModesJson = null;
                if (request.RuleDto.ApplicableTransportModes != null && request.RuleDto.ApplicableTransportModes.Any())
                {
                    transportModesJson = JsonSerializer.Serialize(request.RuleDto.ApplicableTransportModes);
                }

                var rule = new SurchargeRule
                {
                    Id = Guid.NewGuid(),
                    SurchargeId = request.SurchargeId,
                    Name = request.RuleDto.Name,
                    ConditionsJson = request.RuleDto.ConditionsJson ?? "{}",
                    ValidFrom = request.RuleDto.ValidFrom,
                    ValidTo = request.RuleDto.ValidTo,
                    ZoneFromId = request.RuleDto.ZoneFromId,
                    ZoneToId = request.RuleDto.ZoneToId,
                    ApplicableTransportModes = transportModesJson,
                    OverrideValue = request.RuleDto.OverrideValue,
                    Priority = request.RuleDto.Priority,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                await _surchargeRepository.AddRuleAsync(rule);
                await _surchargeRepository.SaveChange(cancellationToken);

                // Get created rule with details
                var createdRule = await _surchargeRepository.GetRuleByIdWithDetailsAsync(rule.Id, cancellationToken);
                var result = _mapper.Map<SurchargeRuleDTO>(createdRule);

                return new ResponseHttp
                {
                    Resultat = result,
                    Status = StatusCodes.Status201Created
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