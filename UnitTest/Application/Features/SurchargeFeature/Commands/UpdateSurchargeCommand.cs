// Application/Features/SurchargeFeature/Commands/UpdateSurchargeCommand.cs
using Application.Features.SurchargeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SurchargeFeature.Commands
{
    public record UpdateSurchargeCommand(Guid Id, UpdateSurchargeDTO SurchargeDto) : IRequest<ResponseHttp>;

    public class UpdateSurchargeCommandHandler : IRequestHandler<UpdateSurchargeCommand, ResponseHttp>
    {
        private readonly ISurchargeRepository _surchargeRepository;
        private readonly IMapper _mapper;

        public UpdateSurchargeCommandHandler(ISurchargeRepository surchargeRepository, IMapper mapper)
        {
            _surchargeRepository = surchargeRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateSurchargeCommand request, CancellationToken cancellationToken)
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

                // Parse enums
                if (!Enum.TryParse<SurchargeType>(request.SurchargeDto.Type, true, out var type))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid surcharge type",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                if (!Enum.TryParse<CalculationType>(request.SurchargeDto.CalculationType, true, out var calculationType))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid calculation type",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                surcharge.Name = request.SurchargeDto.Name;
                surcharge.Description = request.SurchargeDto.Description;
                surcharge.Type = type;
                surcharge.CalculationType = calculationType;
                surcharge.Value = request.SurchargeDto.Value;
                surcharge.IsActive = request.SurchargeDto.IsActive;
                surcharge.ModifiedDate = DateTime.UtcNow;

                await _surchargeRepository.Update(surcharge);
                await _surchargeRepository.SaveChange(cancellationToken);

                var rules = await _surchargeRepository.GetRulesBySurchargeIdAsync(surcharge.Id, cancellationToken);

                var result = _mapper.Map<SurchargeDTO>(surcharge);
                result.RulesCount = rules.Count();

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