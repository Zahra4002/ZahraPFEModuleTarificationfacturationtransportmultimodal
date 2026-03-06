using Application.Setting;
using MediatR;
using System;

namespace Application.Features.DashboardFeature.Queries
{
    public record GetRevenueStatsQuery(DateTime? From, DateTime? To) : IRequest<ResponseHttp>;
}