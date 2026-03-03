// Application/Features/TariffGridFeature/Commands/CloneTariffGridCommand.cs
using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Commands
{
    public record CloneTariffGridCommand(Guid Id, string? NewCode = null, string? NewName = null) : IRequest<ResponseHttp>;

    public class CloneTariffGridCommandHandler : IRequestHandler<CloneTariffGridCommand, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public CloneTariffGridCommandHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CloneTariffGridCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sourceGrid = await _tariffGridRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);
                if (sourceGrid == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Tariff grid not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Generate new code if not provided
                var newCode = request.NewCode ?? $"{sourceGrid.Code}-COPY";

                // Check if code already exists
                var existingGrid = await _tariffGridRepository.GetByCodeAsync(newCode);
                if (existingGrid != null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"Tariff grid with code '{newCode}' already exists",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                var clonedGrid = await _tariffGridRepository.CloneGridAsync(request.Id, newCode, request.NewName);
                await _tariffGridRepository.SaveChange(cancellationToken);

                var result = _mapper.Map<TariffGridDTO>(clonedGrid);
                result.TariffLinesCount = clonedGrid.TariffLines?.Count ?? 0;

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