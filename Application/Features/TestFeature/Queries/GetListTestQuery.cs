using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TestFeature.Queries
{
    public class GetListTestQuery : IRequest<ResponseHttp>
    {
        public class GetListTestQueryHandler : IRequestHandler<GetListTestQuery, ResponseHttp>
        {
            private readonly ICleanArchitecturContext _cleanContext;

            public GetListTestQueryHandler(ICleanArchitecturContext cleantContext)
            {
                _cleanContext = cleantContext;
            }
            public async Task<ResponseHttp> Handle(GetListTestQuery request, CancellationToken cancellationToken)
            {
                var tests = await _cleanContext.Tests.Where(x => x.IsDeleted == false).ToListAsync(cancellationToken);
                return new ResponseHttp
                {
                    Status = 200,
                    Fail_Messages = "None",
                    Resultat = new
                    {
                        Tests = tests
                    }
                };
            }
        }
    }
}
