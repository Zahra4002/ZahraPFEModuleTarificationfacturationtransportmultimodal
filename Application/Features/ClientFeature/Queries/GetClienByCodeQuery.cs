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
    public class GetClienByCodeQuery : IRequest<ResponseHttp>
    {
        public string Code { get; set; }

        public GetClienByCodeQuery(string code)
        {
            Code = code ;
        }

        public class GetNavigatorByCodeQueryHandler : IRequestHandler<GetClienByCodeQuery, ResponseHttp>
        {
            private readonly ICleanArchitecturContext _trackingContext;

            public GetNavigatorByCodeQueryHandler(ICleanArchitecturContext trackingContext)
            {
                _trackingContext = trackingContext;
            }
            public async Task<ResponseHttp> Handle(GetClienByCodeQuery request, CancellationToken cancellationToken)
            {
                var naviagtor = await _trackingContext.Clients
                    .Where(x => x.Code == request.Code)
                    .SingleOrDefaultAsync(cancellationToken);
                if (naviagtor == null)
                    return new ResponseHttp()
                    {
                        Resultat = "Not Found",
                        Status = 404,
                        Fail_Messages = "NoT Exist a Client with this Id"
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
