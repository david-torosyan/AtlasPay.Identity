
using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Domain.Entities;

namespace AtlasPay.Identity.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(
    IdentityDbContext dbContext) : IUserRepository
{
    public void Add(User user)
        => dbContext.Users.Add(user);
}
