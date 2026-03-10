using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
using MediatR;

namespace Application.Features.ContractFeature.Queries
{
    public record RenewContractCommand(Guid ContractId) : IRequest<ResponseHttp>;

    public class RenewContractCommandHandler : IRequestHandler<RenewContractCommand, ResponseHttp>
    {
        private readonly IContractRepository _repository;

        public RenewContractCommandHandler(IContractRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseHttp> Handle(RenewContractCommand request, CancellationToken ct)
        {
            var contract = await _repository.GetByIdAsync(request.ContractId, ct);

            if (contract == null)
                return new ResponseHttp { Status = 404, Fail_Messages = "Contrat non trouvé" };

            if (!contract.AutoRenew)
                return new ResponseHttp { Status = 400, Fail_Messages = "Le renouvellement automatique n'est pas activé" };

            // Logique de renouvellement (exemple simple)
            contract.ValidFrom = contract.ValidTo.AddDays(1);
            contract.ValidTo = contract.ValidTo.AddYears(1); 

            await _repository.UpdateAsync(contract, ct);
            await _repository.SaveChangesAsync(ct);

            return new ResponseHttp { Status = 200, Resultat = "Contrat renouvelé avec succès" };
        }
    }
}
