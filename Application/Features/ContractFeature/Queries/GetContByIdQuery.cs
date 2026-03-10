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
    public class GetContractByIdQuery : IRequest<ResponseHttp>
    {
        public Guid Id { get; set; }

        public GetContractByIdQuery(Guid id)
        {
            Id = id;
        }

        public class GetNavigatorByIdQueryHandler : IRequestHandler<GetContractByIdQuery, ResponseHttp>
        {
            private readonly ICleanArchitecturContext _trackingContext;

            public GetNavigatorByIdQueryHandler(ICleanArchitecturContext trackingContext)
            {
                _trackingContext = trackingContext;
            }
            public async Task<ResponseHttp> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
            {
                var naviagtor = await _trackingContext.Contracts
                    .Where(x => x.Id == request.Id)
                    .SingleOrDefaultAsync(cancellationToken);
                if (naviagtor == null)
                    return new ResponseHttp()
                    {
                        Resultat = "Not Found",
                        Status = 404,
                        Fail_Messages = "NoT Exist a Contract with this Id"
                    };
                return new ResponseHttp()
                {
                    Resultat = naviagtor,
                    Status = 200,
                    Fail_Messages = "None"
                };
            }
        }
    }
}
