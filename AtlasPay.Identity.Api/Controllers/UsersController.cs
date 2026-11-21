
using AtlasPay.Identity.Api.Contracts.Users;
using AtlasPay.Identity.Application.Features.Users.Commands;
using AtlasPay.Identity.Application.Features.Users.Commands.RevokeUserSessions;
using AtlasPay.Identity.Application.Features.Users.Commands.SendAccountActivation;
using AtlasPay.Identity.Application.Features.Users.Commands.SetUserEnabled;
using AtlasPay.Identity.Application.Features.Users.Queries.GetUserByCustomerId;
using AtlasPay.Identity.Application.Features.Users.Queries.GetUserByKeycloakId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AtlasPay.Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [Authorize(Policy = "IdentityProvisioning")]
    [HttpPost]
    public async Task<ActionResult<Guid>> Register(
        [FromBody] RegisterUserRequest request,
        [FromServices] RegisterUserHandler handler,
        CancellationToken ct)
    {
        var command = new RegisterUserCommand(
            request.RequestId,
            request.CustomerId,
            request.Email,
            request.FirstName,
            request.LastName);

        var userId = await handler.HandleAsync(command, ct);

        return Ok(userId);
    }

    [Authorize(Policy = "IdentityRead")]
    [HttpGet("by-customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomerId(
    Guid customerId,
    [FromServices] GetUserByCustomerIdHandler handler,
    CancellationToken ct)
    {
        var user = await handler.HandleAsync(
            new GetUserByCustomerIdQuery(customerId), ct);

        return user is null ? NotFound() : Ok(user);
    }

    [Authorize(Policy = "IdentityUserManagement")]
    [HttpPatch("by-customer/{customerId:guid}/disable")]
    public async Task<IActionResult> Disable(
    Guid customerId,
    [FromServices] SetUserEnabledHandler handler,
    CancellationToken ct)
    {
        var found = await handler.HandleAsync(
            new SetUserEnabledCommand(customerId, false), ct);

        return found ? NoContent() : NotFound();
    }

    [Authorize(Policy = "IdentityUserManagement")]
    [HttpPatch("by-customer/{customerId:guid}/enable")]
    public async Task<IActionResult> Enable(
        Guid customerId,
        [FromServices] SetUserEnabledHandler handler,
        CancellationToken ct)
    {
        var found = await handler.HandleAsync(
            new SetUserEnabledCommand(customerId, true), ct);

        return found ? NoContent() : NotFound();
    }

    [Authorize(Policy = "IdentityProvisioning")]
    [HttpPost("by-customer/{customerId:guid}/activation-email")]
    [EnableRateLimiting("ActivationEmail")]
    public async Task<IActionResult> SendActivationEmail(
        Guid customerId,
        [FromServices] SendAccountActivationHandler handler,
        CancellationToken ct)
    {
        var found = await handler.HandleAsync(
            new SendAccountActivationCommand(customerId), ct);

        return found ? NoContent() : NotFound();
    }

    [HttpGet("by-keycloak/{keycloakUserId}")]
    [Authorize(Policy = "IdentityRead")]
    public async Task<IActionResult> GetByKeycloakId(
    string keycloakUserId,
        [FromServices] GetUserByKeycloakIdHandler handler,
        CancellationToken ct)
    {
        var user = await handler.HandleAsync(
            new GetUserByKeycloakIdQuery(keycloakUserId), ct);

        return user is null ? NotFound() : Ok(user);
    }

    [Authorize(Policy = "IdentityUserManagement")]
    [HttpPost("by-customer/{customerId:guid}/revoke-sessions")]
    public async Task<IActionResult> RevokeSessions(
    Guid customerId,
    [FromServices] RevokeUserSessionsHandler handler,
    CancellationToken ct)
    {
        var found = await handler.HandleAsync(
            new RevokeUserSessionsCommand(customerId), ct);

        return found ? NoContent() : NotFound();
    }
}
