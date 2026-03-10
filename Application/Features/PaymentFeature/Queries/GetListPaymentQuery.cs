using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.PaymentFeature.Queries
{
    public class GetListPaymentQuery : IRequest<ResponseHttp>
    {
        public class GetListPaymentQueryHandler : IRequestHandler<GetListPaymentQuery, ResponseHttp>
        {
            private readonly ICleanArchitecturContext _cleanContext;

            public GetListPaymentQueryHandler(ICleanArchitecturContext cleantContext)
            {
                _cleanContext = cleantContext;
            }
            public async Task<ResponseHttp> Handle(GetListPaymentQuery request, CancellationToken cancellationToken)
            {
                var payments = await _cleanContext.Payments.Where(x => x.IsDeleted == false).ToListAsync(cancellationToken);
                return new ResponseHttp
                {
                    Status = 200,
                    Fail_Messages = "None",
                    Resultat = new
                    {
                        Payments = payments
                    }
                };
            }
        }
    }
}
