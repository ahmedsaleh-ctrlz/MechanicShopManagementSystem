using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery : ICachedQuery<Result<List<CustomerDto>>>
{
    public string CacheKey => "customers";

    public string[] Tags => ["customers"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}

public class GetCustomersQueryHandler(IAppDbContext context) : IRequestHandler<GetCustomersQuery, Result<List<CustomerDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        return await _context.Customers.Include(c => c.Vehicles)
                                       .AsNoTracking()
                                       .Select(c => c.ToDto())
                                       .ToListAsync(ct);
    }
}