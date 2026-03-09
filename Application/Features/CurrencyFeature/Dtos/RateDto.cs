using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CurrencyFeature.Dtos
{
    public class RateDto
    {
        public Guid Id { get; set; }
        public Guid FromCurrencyId { get; set; }
        public string FromCurrencyCode { get; set; } = string.Empty;
        public Guid ToCurrencyId { get; set; }
        public string ToCurrencyCode { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Source { get; set; }

        public RateDto(ExchangeRate rate )
        {
            Id = rate.Id;
            FromCurrencyId = rate.FromCurrencyId;
            FromCurrencyCode = rate.FromCurrency.Code;
            ToCurrencyId = rate.ToCurrencyId;
            ToCurrencyCode = rate.ToCurrency.Code;
            Rate = rate.Rate;
            EffectiveDate = rate.EffectiveDate;
            ExpiryDate = rate.ExpiryDate;
            Source = rate.Source;

        }

    }
}
