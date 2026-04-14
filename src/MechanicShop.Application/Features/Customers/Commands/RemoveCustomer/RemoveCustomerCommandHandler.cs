using FluentValidation;
using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.RemoveCustomer;

public class RemoveCustomerCommandHandler(IAppDbContext context,
    ILogger<RemoveCustomerCommandHandler> logger,
    HybridCache cache) : IRequestHandler<RemoveCustomerCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<RemoveCustomerCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Deleted>> Handle(RemoveCustomerCommand request, CancellationToken ct)
    {
        var customer = await _context.Customers.FindAsync([request.CustomerId],ct);
        if (customer is null)
        {
            _logger.LogWarning("Customer with id {CustomerId} not found for deletion.", request.CustomerId);
            return ApplicationErrors.CustomerNotFound;
        }

        var IsAssociatedWithWorkOrders = await _context.WorkOrders.Include(wo => wo.Vehicle)
            .Where(wo => wo.Vehicle != null)
            .AnyAsync(v => v.Vehicle!.CustomerId == request.CustomerId);

        if (IsAssociatedWithWorkOrders)
        {
            _logger.LogWarning("Customer {CustomerId} cannot be deleted because they have associated work orders (past, scheduled, or in-progress).", request.CustomerId);
            return CustomerErrors.CannotDeleteCustomerWithWorkOrders;
        }

        _context.Customers.Remove(customer);

        await _context.SaveChangesAsync(ct);

        await _cache.RemoveByTagAsync("customer", ct);

        _logger.LogInformation("Customer {CustomerId} deleted successfully.", request.CustomerId);

        return Result.Deleted;

    }
}