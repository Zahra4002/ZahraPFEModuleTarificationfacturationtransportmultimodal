using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CurrencyFeature.Dtos
{
    public class CurrencyDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty; // ISO 4217 (EUR, USD, TND)
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public int DecimalPlaces { get; set; } = 2;
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;

        // Constructeur sans paramètres pour AutoMapper
        public CurrencyDto()
        {
        }

        public CurrencyDto(Currency currency) {
            
            Id = currency.Id;
            Code = currency.Code;
            Name = currency.Name;
            Symbol = currency.Symbol;
            DecimalPlaces = currency.DecimalPlaces;
            IsActive = currency.IsActive;
            IsDefault = currency.IsDefault;
        }
    }
}
