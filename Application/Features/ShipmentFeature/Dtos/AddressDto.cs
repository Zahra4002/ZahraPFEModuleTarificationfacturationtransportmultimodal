using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Dtos
{
    public class AddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? State { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Ajoutez ce constructeur par défaut
        public AddressDto() { }


        public AddressDto(Address address)
        {
            Street = address.Street;
            City = address.City;
            PostalCode = address.PostalCode;
            Country = address.Country;
            State = address.State;
            Latitude = address.Latitude;
            Longitude = address.Longitude;
        }


        
    }
}
