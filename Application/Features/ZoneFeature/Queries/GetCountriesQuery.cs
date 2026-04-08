using Application.Services;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ZoneFeature.Queries
{
    public record GetCountriesQuery() : IRequest<ResponseHttp>;

    public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, ResponseHttp>
    {
        private readonly ICountryService _countryService;

        public GetCountriesQueryHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<ResponseHttp> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var countries = await _countryService.GetAllCountries();

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = countries
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = $"Erreur lors de la récupération des pays: {ex.Message}"
                };
            }
        }
    }
}