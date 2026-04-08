using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class VatRateInfo
    {
        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public decimal StandardRate { get; set; }
        public decimal? ReducedRate { get; set; }
    }

    public interface IVatRateService
    {
        Task<decimal?> GetVatRate(string countryCode, CancellationToken cancellationToken = default);
        Task<VatRateInfo?> GetVatInfo(string countryCode, CancellationToken cancellationToken = default);
        Task<List<VatRateInfo>> GetAllVatRates(CancellationToken cancellationToken = default);
    }

    public class VatSenseService : IVatRateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public VatSenseService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["VatSense:ApiKey"] ?? "970c1eaa1ce2d8f2fa7601ff67724d29";
            _httpClient.BaseAddress = new Uri("https://api.vatsense.com/v1/");
        }

        public async Task<decimal?> GetVatRate(string countryCode, CancellationToken cancellationToken = default)
        {
            var url = $"rate?country_code={countryCode.ToUpper()}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-API-Key", _apiKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            // DEBUG - retirer après diagnostic
            throw new Exception($"Status: {response.StatusCode} | ApiKey: {_apiKey[..6]}... | Body: {rawJson}");
        }

        public async Task<VatRateInfo?> GetVatInfo(string countryCode, CancellationToken cancellationToken = default)
        {
            var url = $"rate?country_code={countryCode.ToUpper()}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-API-Key", _apiKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            // DEBUG - retirer après diagnostic
            throw new Exception($"Status: {response.StatusCode} | ApiKey: {_apiKey[..6]}... | Body: {rawJson}");
        }

        public async Task<List<VatRateInfo>> GetAllVatRates(CancellationToken cancellationToken = default)
        {
            var countryCodes = new List<string>
            {
                "TN","FR","DE","IT","ES","BE","NL","PT","PL","SE","DK","FI",
                "NO","CH","GB","US","MA","DZ","EG","SA","AE","TR","CN","JP",
                "IN","BR","CA","AU","ZA","NG","SN","CI","CM","LY","MR","LB",
                "JO","KW","QA","AT","CZ","HU","RO","BG","HR","SK","SI","GR",
                "IE","LU","MT","CY","EE","LV","LT"
            };

            var result = new List<VatRateInfo>();

            foreach (var code in countryCodes)
            {
                if (cancellationToken.IsCancellationRequested) break;
                try
                {
                    var url = $"rate?country_code={code}";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Add("X-API-Key", _apiKey);

                    var response = await _httpClient.SendAsync(request, cancellationToken);
                    var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (!response.IsSuccessStatusCode) continue;

                    var data = await System.Text.Json.JsonSerializer.DeserializeAsync<VatSenseRateResponse>(
                        await response.Content.ReadAsStreamAsync(cancellationToken),
                        cancellationToken: cancellationToken
                    );

                    if (data != null && data.StandardRate > 0)
                    {
                        result.Add(new VatRateInfo
                        {
                            CountryCode = code,
                            CountryName = data.Country ?? code,
                            StandardRate = data.StandardRate,
                            ReducedRate = data.ReducedRate
                        });
                    }

                    await Task.Delay(200, cancellationToken);
                }
                catch
                {
                    continue;
                }
            }

            return result;
        }
    }

    public class VatSenseRateResponse
    {
        public string Country { get; set; } = string.Empty;
        public decimal StandardRate { get; set; }
        public decimal? ReducedRate { get; set; }
    }
}