FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY *.sln .

COPY src/Account/Account.Domain/*.csproj ./src/Account/Account.Domain/

COPY src/Account/Account.API/*.csproj ./src/Account/Account.API/
COPY src/Account/Account.Application/*.csproj ./src/Account/Account.Application/
COPY src/Account/Account.Domain/*.csproj ./src/Account/Account.Domain/
COPY src/Account/Account.Infrastructure/*.csproj ./src/Account/Account.Infrastructure/
COPY src/Transfers/Transfers.API/*.csproj ./src/Transfers/Transfers.API/
COPY src/Transfers/Transfers.Application/*.csproj ./src/Transfers/Transfers.Application/
COPY src/Transfers/Transfers.Domain/*.csproj ./src/Transfers/Transfers.Domain/
COPY src/Transfers/Transfers.Infrastructure/*.csproj ./src/Transfers/Transfers.Infrastructure/
COPY tests/Account.API.IntegrationTests/*.csproj ./tests/Account.API.IntegrationTests/
COPY tests/Account.Application.UnitTests/*.csproj ./tests/Account.Application.UnitTests/

RUN dotnet restore BankMore.sln

COPY . .
WORKDIR "/source/src/Transfers/Transfers.API"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Transfers.API.dll"]