using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ClientFeature.Queries
{
    public class GetListClientQuery : IRequest<ResponseHttp>
    {
        public class GetListClientQueryHandler : IRequestHandler<GetListClientQuery, ResponseHttp>
        {
            private readonly ICleanArchitecturContext _cleanContext;

            public GetListClientQueryHandler(ICleanArchitecturContext cleantContext)
            {
                _cleanContext = cleantContext;
            }
            public async Task<ResponseHttp> Handle(GetListClientQuery request, CancellationToken cancellationToken)
            {
                var clients = await _cleanContext.Clients.Where(x => x.IsDeleted == false).ToListAsync(cancellationToken);
                return new ResponseHttp
                {
                    Status = 200,
                    Fail_Messages = "None",
                    Resultat = new
                    {
                        Clients = clients
                    }
                };
            }
        }
    }
}
