using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ContractFeature.Queries
{
    public class GetListContractQuery : IRequest<ResponseHttp>
    {
        public class GetListContractQueryHandler : IRequestHandler<GetListContractQuery, ResponseHttp>
        {
            private readonly ICleanArchitecturContext _cleanContext;

            public GetListContractQueryHandler(ICleanArchitecturContext cleantContext)
            {
                _cleanContext = cleantContext;
            }
            public async Task<ResponseHttp> Handle(GetListContractQuery request, CancellationToken cancellationToken)
            {
                var contracts = await _cleanContext.Contracts.Where(x => x.IsDeleted == false).ToListAsync(cancellationToken);
                return new ResponseHttp
                {
                    Status = 200,
                    Fail_Messages = "None",
                    Resultat = new
                    {
                        Contracts = contracts
                    }
                };
            }
        }
    }
}
