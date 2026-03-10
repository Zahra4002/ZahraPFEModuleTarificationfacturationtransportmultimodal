using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.TestFeature.Commands;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ClientFeature.Commands
{
    public record DeleteClientCommandNew(
        Guid ClientId
        )
        : IRequest<ResponseHttp>
    {

        public class DeleteClientCommandNewHandler : IRequestHandler<DeleteClientCommandNew, ResponseHttp>
        {
            private readonly IClientRepository clientRepository;
            public DeleteClientCommandNewHandler(IClientRepository clientRepository)
            {
                this.clientRepository = clientRepository;
            }

            public async Task<ResponseHttp> Handle(DeleteClientCommandNew request, CancellationToken cancellationToken)
            {
                var client = await clientRepository.GetById(request.ClientId);

                if (client == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "No test found",
                        Status = StatusCodes.Status400BadRequest,
                    };
                }

                await clientRepository.SoftDelete(request.ClientId);
                await clientRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                };
            }
        }
    }
}
