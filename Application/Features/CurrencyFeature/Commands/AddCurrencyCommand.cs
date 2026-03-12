using Application.Common.Validator;
using Application.Features.CurrencyFeature.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Application.Setting;
using MediatR;
using Application.Interfaces;
using Domain.Entities;
using Application.Features.CurrencyFeature.Dtos;

namespace Application.Features.CurrencyFeature.Commands
{
    public record AddCurrencyCommand(
        string Code,
        string Name,
        string Symbol,
        int DecimalPlaces,
        bool IsActive,
        bool IsDefault
        ) : IRequest<ResponseHttp>
    {
        public class AddCurrencyCommandHandler : IRequestHandler<AddCurrencyCommand, ResponseHttp>
        {
            private readonly ICurrencyRepository _currencyRepository;
            private readonly IMapper _mapper;

            public AddCurrencyCommandHandler(ICurrencyRepository currencyRepository, IMapper mapper)
            {
                _currencyRepository = currencyRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(AddCurrencyCommand request, CancellationToken cancellationToken)
            {
                var newCurrency = _mapper.Map<Currency>(request);
                newCurrency.Id = Guid.NewGuid();

                await _currencyRepository.Post(newCurrency);
                await _currencyRepository.SaveChange(cancellationToken);
                
                var currencyDto = _mapper.Map<CurrencyDto>(newCurrency);
                
                return new ResponseHttp
                {
                    Resultat = currencyDto, 
                    Status = 201, 
                    Fail_Messages = "Currency created successfully"
                };
            }
        }
    }
}
