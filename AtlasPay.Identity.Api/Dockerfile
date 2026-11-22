FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AtlasPay.Identity.Api/*.csproj AtlasPay.Identity.Api/
COPY AtlasPay.Identity.Application/*.csproj AtlasPay.Identity.Application/
COPY AtlasPay.Identity.Domain/*.csproj AtlasPay.Identity.Domain/
COPY AtlasPay.Identity.Infrastructure/*.csproj AtlasPay.Identity.Infrastructure/

RUN dotnet restore AtlasPay.Identity.Api/AtlasPay.Identity.Api.csproj

COPY AtlasPay.Identity.Api/ AtlasPay.Identity.Api/
COPY AtlasPay.Identity.Application/ AtlasPay.Identity.Application/
COPY AtlasPay.Identity.Domain/ AtlasPay.Identity.Domain/
COPY AtlasPay.Identity.Infrastructure/ AtlasPay.Identity.Infrastructure/

RUN dotnet publish AtlasPay.Identity.Api/AtlasPay.Identity.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "AtlasPay.Identity.Api.dll"]