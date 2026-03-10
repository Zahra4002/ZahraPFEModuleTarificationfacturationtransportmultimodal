using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public record AddClientCommandNew(
        string code,
        string name,
        string taxId,
        string email,
        string phoneNumber,
        UserRole UserRole,
        Address bullingAddress,
        Address shippingAddress,
        string defaultCurrencyCode,
        decimal creditLimit,
        int paymenttermDays,
        bool isActive
        ) : IRequest<ResponseHttp>
    {
        public class AddClientCommandNewHandler : IRequestHandler<AddClientCommandNew, ResponseHttp>
        {
            private readonly IClientRepository clientRepository;
            private readonly IMapper _mapper;

            public AddClientCommandNewHandler(IClientRepository clientRepository, IMapper mapper)
            {
                this.clientRepository = clientRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(AddClientCommandNew request, CancellationToken cancellationToken)
            {
                try
                {
                    var Client = _mapper.Map<Client>(request);

                    Client = await clientRepository.Post(Client);
                    await clientRepository.SaveChange(cancellationToken);

                    return new ResponseHttp()
                    {
                        Resultat = _mapper.Map<ClientDTO>(Client),
                        Status = 200
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = ex.Message,
                        Status = StatusCodes.Status400BadRequest,
                    };
                }
            }
        }
    }
}
