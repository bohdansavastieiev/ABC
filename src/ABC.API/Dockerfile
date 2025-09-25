FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/ABC.API/ABC.API.csproj", "src/ABC.API/"]
COPY ["src/ABC.Application/ABC.Application.csproj", "src/ABC.Application/"]
COPY ["src/ABC.Domain/ABC.Domain.csproj", "src/ABC.Domain/"]
COPY ["src/ABC.Infrastructure/ABC.Infrastructure.csproj", "src/ABC.Infrastructure/"]
RUN dotnet restore "src/ABC.API/ABC.API.csproj"

COPY . .

WORKDIR "/src/src/ABC.API"
RUN dotnet publish "ABC.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ABC.API.dll"]