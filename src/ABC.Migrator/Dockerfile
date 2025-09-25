FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/ABC.Migrator/ABC.Migrator.csproj", "src/ABC.Migrator/"]
COPY ["src/ABC.Infrastructure/ABC.Infrastructure.csproj", "src/ABC.Infrastructure/"]
COPY ["src/ABC.Application/ABC.Application.csproj", "src/ABC.Application/"]
COPY ["src/ABC.Domain/ABC.Domain.csproj", "src/ABC.Domain/"]

RUN dotnet restore "src/ABC.Migrator/ABC.Migrator.csproj"

COPY . .
WORKDIR "/src/src/ABC.Migrator"
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ABC.Migrator.dll"]