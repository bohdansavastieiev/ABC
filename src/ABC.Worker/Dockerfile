FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/ABC.Worker/ABC.Worker.csproj", "src/ABC.Worker/"]
COPY ["src/ABC.Application/ABC.Application.csproj", "src/ABC.Application/"]
COPY ["src/ABC.Domain/ABC.Domain.csproj", "src/ABC.Domain/"]
COPY ["src/ABC.Infrastructure/ABC.Infrastructure.csproj", "src/ABC.Infrastructure/"]
RUN dotnet restore "src/ABC.Worker/ABC.Worker.csproj"

COPY . .

WORKDIR "/src/src/ABC.Worker"
RUN dotnet publish "ABC.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ABC.Worker.dll"]