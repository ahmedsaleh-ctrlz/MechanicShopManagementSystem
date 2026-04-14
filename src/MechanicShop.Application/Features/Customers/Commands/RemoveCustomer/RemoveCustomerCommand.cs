using MechanicShop.Domain.Common.Results;
using MediatR;
using System.Net.WebSockets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MechanicShop.Application.Features.Customers.Commands.RemoveCustomer;
public sealed record RemoveCustomerCommand(Guid CustomerId) : IRequest<Result<Deleted>>;
