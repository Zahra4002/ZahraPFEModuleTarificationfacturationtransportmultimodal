using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Application.Services
{
    public class CountryInfo
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Flag { get; set; } = string.Empty;
    }

    public interface ICountryService
    {
        Task<List<CountryInfo>> GetAllCountries();
    }

    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;

        public CountryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://restcountries.com/v3.1/");
        }

        public async Task<List<CountryInfo>> GetAllCountries()
        {
            var response = await _httpClient.GetAsync("all?fields=name,cca2,flags");
            response.EnsureSuccessStatusCode();

            var countries = await response.Content.ReadFromJsonAsync<List<CountryApiResponse>>();
            var result = new List<CountryInfo>();

            foreach (var country in countries ?? new List<CountryApiResponse>())
            {
                // Correction: utiliser "emoji" (minuscule) ou vérifier les deux cas
                // APRÈS
                string flag = country.Flags?.Emoji ?? "";

                result.Add(new CountryInfo
                {
                    Code = country.Cca2,
                    Name = country.Name?.Common ?? country.Cca2,
                    Flag = flag
                });
            }

            result.Sort((a, b) => a.Name.CompareTo(b.Name));
            return result;
        }
    }

    // Classes pour la désérialisation JSON
    public class CountryApiResponse
    {
        public NameInfo? Name { get; set; }
        public string Cca2 { get; set; } = string.Empty;
        public FlagInfo? Flags { get; set; }
    }

    public class NameInfo
    {
        public string Common { get; set; } = string.Empty;
    }

    // APRÈS
    public class FlagInfo
    {
        [JsonPropertyName("emoji")]
        public string Emoji { get; set; } = string.Empty;
    }
}