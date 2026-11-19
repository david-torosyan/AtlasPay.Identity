using AtlasPay.Identity.Api.Extensions;
using AtlasPay.Identity.Application.Features.Users.Commands;
using AtlasPay.Identity.Application.Features.Users.Commands.SendAccountActivation;
using AtlasPay.Identity.Application.Features.Users.Commands.SetUserEnabled;
using AtlasPay.Identity.Application.Features.Users.Queries.GetUserByCustomerId;
using AtlasPay.Identity.Application.Features.Users.Queries.GetUserByKeycloakId;
using AtlasPay.Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<GetUserByCustomerIdHandler>();
builder.Services.AddScoped<SetUserEnabledHandler>();
builder.Services.AddScoped<SendAccountActivationHandler>();
builder.Services.AddScoped<GetUserByKeycloakIdHandler>();

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
