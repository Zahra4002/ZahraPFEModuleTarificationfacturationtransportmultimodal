using Application.Features.SurchargeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SurchargeFeature.Commands
{
    public record CreateSurchargeCommand(CreateSurchargeDTO SurchargeDto) : IRequest<ResponseHttp>;

    public class CreateSurchargeCommandHandler : IRequestHandler<CreateSurchargeCommand, ResponseHttp>
    {
        private readonly ISurchargeRepository _surchargeRepository;
        private readonly IMapper _mapper;

        public CreateSurchargeCommandHandler(ISurchargeRepository surchargeRepository, IMapper mapper)
        {
            _surchargeRepository = surchargeRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateSurchargeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if code already exists
                var exists = await _surchargeRepository.CodeExistsAsync(request.SurchargeDto.Code);
                if (exists)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"Surcharge with code '{request.SurchargeDto.Code}' already exists",
                        Status = StatusCodes.Status400BadRequest
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

                var surcharge = new Surcharge
                {
                    Id = Guid.NewGuid(),
                    Code = request.SurchargeDto.Code,
                    Name = request.SurchargeDto.Name,
                    Description = request.SurchargeDto.Description,
                    Type = type,
                    CalculationType = calculationType,
                    Value = request.SurchargeDto.Value,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                await _surchargeRepository.Post(surcharge);
                await _surchargeRepository.SaveChange(cancellationToken);

                var result = _mapper.Map<SurchargeDTO>(surcharge);
                result.RulesCount = 0;

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