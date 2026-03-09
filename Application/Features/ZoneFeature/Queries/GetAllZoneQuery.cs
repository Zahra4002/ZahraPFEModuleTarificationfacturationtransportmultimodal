using Application.Features.ZoneFeature.Dtos;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ZoneFeature.Queries
{
    public record GetAllZoneQuery
        (
        int ? PageNumber,
        int ? PageSize,
        string? SortedBy,
        bool SortDescending,
        string? SearchTerm
        )
    
        : IRequest<List<ZoneDto>>
    {
        public class GetAllZonesByQueryHandler : IRequestHandler<GetAllZoneQuery, List<ZoneDto>>
        {
            private readonly IZoneRepository _zoneRepository;
            public GetAllZonesByQueryHandler(IZoneRepository zoneRepository)
            {
                _zoneRepository = zoneRepository;
            }
            public async Task<List<ZoneDto>> Handle(GetAllZoneQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var zones = await _zoneRepository.GetAllWithTypesAsync(request.PageNumber,request.PageSize,request.SortedBy,request.SortDescending,request.SearchTerm,cancellationToken);

                    if (zones == null || zones.Count == 0)
                    {
                        return new List<ZoneDto>();
                    }

                    var zonesResult = zones.Select(z => new ZoneDto(z)).ToList();

                    return zonesResult;
                }
                catch (Exception ex)
                {
                    // Log the exception if needed
                    throw new ApplicationException("An error occurred while retrieving zones.", ex);
                }
            }
        }
    }
}