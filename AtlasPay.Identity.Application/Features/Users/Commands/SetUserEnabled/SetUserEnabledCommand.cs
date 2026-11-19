namespace AtlasPay.Identity.Application.Features.Users.Commands.SetUserEnabled;

public sealed record SetUserEnabledCommand(
    Guid CustomerId,
    bool Enabled);