using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.TestFeature.Commands;
using Application.Features.TestFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ClientFeature.Commands
{
    public record UpdateClientCommandNew(
        Guid ClientId,
        string code,
        string name,
        string taxId,
        string email,
        string phoneNumber,
        Address bullingAddress,
        Address shippingAddress,
        string defaultCurrencyCode,
        decimal creditLimit,
        int paymenttermDays,
        bool isActive
        ) : IRequest<ResponseHttp>
    {
        public class UpdateClientCommandNewHandler : IRequestHandler<UpdateClientCommandNew, ResponseHttp>
        {
            private readonly IClientRepository clientRepository;
            private readonly IMapper _mapper;

            public UpdateClientCommandNewHandler(IClientRepository clientRepository, IMapper mapper)
            {
                this.clientRepository = clientRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(UpdateClientCommandNew request, CancellationToken cancellationToken)
            {
                Client? client = await clientRepository.GetById(request.ClientId);

                if (client == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Fail_Messages = "Customer with this Id not found.",
                        Status = StatusCodes.Status400BadRequest,
                    };
                }

                // Utiliser AutoMapper pour mapper les propriétés du request vers l'entité existante
                _mapper.Map(request, client);
                
                await clientRepository.Update(client);
                await clientRepository.SaveChange(cancellationToken);

                var customerToReturn = _mapper.Map<ClientDTO>(client);
                return new ResponseHttp
                {
                    Resultat = customerToReturn,
                    Status = StatusCodes.Status200OK,
                };

            }
        }
    }
}
