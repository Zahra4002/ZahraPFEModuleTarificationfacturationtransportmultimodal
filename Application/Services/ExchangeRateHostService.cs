using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Application.Services
{
    // Réponse pour /live
    public class ExchangeRateHostLiveResponse
    {
        public bool Success { get; set; }
        public string? Source { get; set; }
        public Dictionary<string, decimal>? Quotes { get; set; }
    }

    // Réponse pour /list
    public class ExchangeRateHostListResponse
    {
        public bool Success { get; set; }
        public Dictionary<string, string>? Currencies { get; set; }
    }

    public interface IExchangeRateHostService
    {
        Task<List<ExchangeRate>> FetchAllRates(string baseCurrency = "EUR", CancellationToken cancellationToken = default);
        Task<List<Currency>> FetchAllCurrencies(CancellationToken cancellationToken = default);
    }

    public class ExchangeRateHostService : IExchangeRateHostService
    {
        private readonly HttpClient _httpClient;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly string _apiKey;

        public ExchangeRateHostService(
            HttpClient httpClient,
            IConfiguration configuration,
            ICurrencyRepository currencyRepository)
        {
            _httpClient = httpClient;
            _currencyRepository = currencyRepository;
            _apiKey = configuration["ExchangeRateHost:ApiKey"] ?? string.Empty;

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("https://api.exchangerate.host");
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
            }
        }

        /// <summary>
        /// Récupère tous les taux de change via /live
        /// </summary>
        public async Task<List<ExchangeRate>> FetchAllRates(string baseCurrency = "EUR", CancellationToken cancellationToken = default)
        {
            var rates = new List<ExchangeRate>();

            try
            {
                // URL correcte selon la documentation
                string url = $"/live?access_key={_apiKey}&source={baseCurrency.ToUpper()}";

                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ExchangeRateHostLiveResponse>(cancellationToken: cancellationToken);

                if (result?.Success == true && result?.Quotes != null)
                {
                    var fromCurrency = await GetOrCreateCurrency(baseCurrency, cancellationToken);

                    foreach (var quote in result.Quotes)
                    {
                        // La clé est comme "EURUSD" - extraire la devise cible (3 derniers caractères)
                        var toCurrencyCode = quote.Key.Length >= 3 ? quote.Key.Substring(3) : quote.Key;

                        var toCurrency = await GetOrCreateCurrency(toCurrencyCode, cancellationToken);

                        var exchangeRate = new ExchangeRate
                        {
                            Id = Guid.NewGuid(),
                            FromCurrencyId = fromCurrency.Id,
                            FromCurrency = fromCurrency,
                            ToCurrencyId = toCurrency.Id,
                            ToCurrency = toCurrency,
                            Rate = quote.Value,
                            EffectiveDate = DateTime.UtcNow,
                            ExpiryDate = DateTime.UtcNow.AddDays(1),
                            Source = "exchangerate.host"
                        };

                        rates.Add(exchangeRate);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des taux: {ex.Message}");
            }

            return rates;
        }

        /// <summary>
        /// Récupère toutes les devises supportées via /list
        /// </summary>
        public async Task<List<Currency>> FetchAllCurrencies(CancellationToken cancellationToken = default)
        {
            var currencies = new List<Currency>();

            try
            {
                string url = $"/list?access_key={_apiKey}";

                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ExchangeRateHostListResponse>(cancellationToken: cancellationToken);

                if (result?.Success == true && result?.Currencies != null)
                {
                    foreach (var currency in result.Currencies)
                    {
                        var newCurrency = new Currency
                        {
                            Id = Guid.NewGuid(),
                            Code = currency.Key,
                            Name = currency.Value,
                            Symbol = GetCurrencySymbol(currency.Key),
                            DecimalPlaces = 2,
                            IsActive = true,
                            IsDefault = currency.Key == "EUR"
                        };

                        currencies.Add(newCurrency);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des devises: {ex.Message}");
            }

            return currencies;
        }

        private async Task<Currency> GetOrCreateCurrency(string currencyCode, CancellationToken cancellationToken)
        {
            var existingCurrency = await _currencyRepository.SelectOneAsync(c => c.Code == currencyCode, cancellationToken);

            if (existingCurrency != null)
                return existingCurrency;

            var newCurrency = new Currency
            {
                Id = Guid.NewGuid(),
                Code = currencyCode,
                Name = GetCurrencyName(currencyCode),
                Symbol = GetCurrencySymbol(currencyCode),
                DecimalPlaces = 2,
                IsActive = true,
                IsDefault = false
            };

            await _currencyRepository.Post(newCurrency);
            await _currencyRepository.SaveChange(cancellationToken);

            return newCurrency;
        }

        private static string GetCurrencyName(string code)
        {
            return code switch
            {
                "EUR" => "Euro",
                "USD" => "US Dollar",
                "GBP" => "British Pound",
                "JPY" => "Japanese Yen",
                "CHF" => "Swiss Franc",
                "CAD" => "Canadian Dollar",
                "AUD" => "Australian Dollar",
                "CNY" => "Chinese Yuan",
                "TND" => "Tunisian Dinar",
                _ => code
            };
        }

        private static string GetCurrencySymbol(string code)
        {
            return code switch
            {
                "EUR" => "€",
                "USD" => "$",
                "GBP" => "£",
                "JPY" => "¥",
                "CHF" => "Fr",
                "CAD" => "C$",
                "AUD" => "A$",
                "CNY" => "¥",
                "TND" => "DT",
                _ => code
            };
        }
    }
}