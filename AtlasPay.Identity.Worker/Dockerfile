FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AtlasPay.Identity.Worker/*.csproj AtlasPay.Identity.Worker/
COPY AtlasPay.Identity.Application/*.csproj AtlasPay.Identity.Application/
COPY AtlasPay.Identity.Infrastructure/*.csproj AtlasPay.Identity.Infrastructure/
COPY AtlasPay.Identity.Domain/*.csproj AtlasPay.Identity.Domain/

RUN dotnet restore AtlasPay.Identity.Worker/AtlasPay.Identity.Worker.csproj

COPY AtlasPay.Identity.Worker/ AtlasPay.Identity.Worker/
COPY AtlasPay.Identity.Application/ AtlasPay.Identity.Application/
COPY AtlasPay.Identity.Infrastructure/ AtlasPay.Identity.Infrastructure/
COPY AtlasPay.Identity.Domain/ AtlasPay.Identity.Domain/

RUN dotnet publish AtlasPay.Identity.Worker/AtlasPay.Identity.Worker.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "AtlasPay.Identity.Worker.dll"]