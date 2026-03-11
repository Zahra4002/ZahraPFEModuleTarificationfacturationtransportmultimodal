using Application.Features.ContractFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ContractFeature.Queries
{
    public record GetContractPricingListByContractId(
        Guid ContractId
    ) : IRequest<ResponseHttp>;

    public class GetContractPricingListByContractIdHandler : IRequestHandler<GetContractPricingListByContractId, ResponseHttp>
    {
        private readonly IContractRepository _repository;
        private readonly IMapper _mapper;

        // ✅ CORRIGER : Injecter le mapper dans le constructeur
        public GetContractPricingListByContractIdHandler(IContractRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetContractPricingListByContractId request, CancellationToken ct)
        {
            try
            {
                // 1. Vérifier que le contrat existe
                var contract = await _repository.GetByIdWithDetailsAsync(request.ContractId, ct);

                if (contract == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Contrat non trouvé"
                    };
                }

                // 2. Récupérer les pricing (déjà inclus dans GetByIdWithDetailsAsync)
                var contractPricings = contract.ContractPricings?.Where(cp => !cp.IsDeleted).ToList() ?? new List<Domain.Entities.ContractPricing>();

                // 3. Mapper vers DTO
                var contractPricingListDto = _mapper.Map<List<ContractPricingDto>>(contractPricings);

                return new ResponseHttp
                {
                    Resultat = contractPricingListDto,
                    Status = StatusCodes.Status200OK
                    
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = $"Erreur lors de la récupération des tarifications : {ex.Message}"
                };
            }
        }
    }
}
