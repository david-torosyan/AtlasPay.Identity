
using AtlasPay.Identity.Api.Contracts.Users;
using AtlasPay.Identity.Application.Features.Users.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtlasPay.Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Policy = "IdentityProvisioning")]
public sealed class UsersController : ControllerBase
{
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
}