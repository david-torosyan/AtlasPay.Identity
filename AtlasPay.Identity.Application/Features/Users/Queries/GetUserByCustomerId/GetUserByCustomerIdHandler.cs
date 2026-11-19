using AtlasPay.Identity.Application.Abstractions.Persistence;

namespace AtlasPay.Identity.Application.Features.Users.Queries.GetUserByCustomerId;

public sealed class GetUserByCustomerIdHandler(
    IUserReadRepository userReadRepository)
{
    public Task<UserResponse?> HandleAsync(
        GetUserByCustomerIdQuery query,
        CancellationToken ct)
        => userReadRepository.GetByCustomerIdAsync(
            query.CustomerId, ct);
}