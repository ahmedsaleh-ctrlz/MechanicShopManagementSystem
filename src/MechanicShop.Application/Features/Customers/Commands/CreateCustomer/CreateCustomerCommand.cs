using FluentValidation;
using MechanicShop.Application.Common.IAppDbContext;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;
public sealed record CreateCustomerCommand(
    string Name,
    string PhoneNumber,
    string Email,
    List<CreateVehicleCommand> Vehicles) 
    : IRequest<Result<CustomerDto>>
{
}

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email")
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
         .NotEmpty().WithMessage("Phone number is required.")
         .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must be 7–15 digits and may start with '+'.");

        RuleFor(x => x.Vehicles)
            .NotNull().WithMessage("Vehicle list cannot be null.")
            .Must(p => p.Count > 0).WithMessage("At least one vehicle is required.");

        RuleForEach(x => x.Vehicles).SetValidator(new CreateVehicleCommandValidator());
    }
}

public sealed class CreateCustomerCommandHandler(
    IAppDbContext context,
    ILogger<CreateCustomerCommandHandler> logger,
    HybridCache cache) : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateCustomerCommandHandler> _logger= logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand command, CancellationToken ct)
    {
        
        var email = command.Email.Trim().ToLower();
        var exists = await _context.Customers.AnyAsync(c=> c.Email! == email,ct);

        if (exists) 
        {
            _logger.LogWarning("Customer creation aborted,Email already Exist");
            return CustomerErrors.CustomerExists;
        }

        //Create All Vehicles To Add In Customer Object
        List<Vehicle> vehicles = [];

        foreach (var v in command.Vehicles)
        {
            var vehicleResult = Vehicle.Create(Guid.NewGuid(), v.Make, v.Model, v.Year, v.LicensePlate);

            if (vehicleResult.IsError)
            {
                return vehicleResult.Errors;
            }

            vehicles.Add(vehicleResult.Value);
        };

        //Create Final Customer Result
       
        var CustomerResult = Customer.Create(Guid.NewGuid(),command.Name,command.PhoneNumber, email, vehicles);

        if (CustomerResult.IsError) 
        {
            return CustomerResult.Errors;
        }

        await _context.Customers.AddAsync(CustomerResult.Value, ct);
        await _cache.RemoveByTagAsync("customer", ct);


        _logger.LogInformation("Customer created successfully. Id: {CustomerId}",CustomerResult.Value.Id);

        return CustomerResult.Value.ToDto();
    }
}
