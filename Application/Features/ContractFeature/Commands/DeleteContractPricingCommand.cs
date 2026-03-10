using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
using MediatR;

namespace Application.Features.ContractFeature.Commands
{
    public record DeleteContractPricingCommand(Guid ContractId, Guid PricingId) : IRequest<ResponseHttp>;

    public class DeleteContractPricingCommandHandler : IRequestHandler<DeleteContractPricingCommand, ResponseHttp>
    {
        private readonly IContractRepository _repository;

        public DeleteContractPricingCommandHandler(IContractRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseHttp> Handle(DeleteContractPricingCommand request, CancellationToken ct)
        {
            var contract = await _repository.GetByIdWithDetailsAsync(request.ContractId, ct);

            if (contract == null)
                return new ResponseHttp { Status = 404, Fail_Messages = "Contrat non trouvé" };

            var pricing = contract.ContractPricings.FirstOrDefault(p => p.Id == request.PricingId);

            if (pricing == null)
                return new ResponseHttp { Status = 404, Fail_Messages = "Tarification non trouvée" };

            contract.ContractPricings.Remove(pricing);

            await _repository.UpdateAsync(contract, ct);
            await _repository.SaveChangesAsync(ct);

            return new ResponseHttp { Status = 200, Resultat = "Tarification supprimée" };
        }
    }
}
