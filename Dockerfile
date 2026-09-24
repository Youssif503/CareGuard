FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY ["CareGuard.API/CareGuard.API.csproj", "CareGuard.API/"]
COPY ["CareGuard.Application/CareGuard.Application.csproj", "CareGuard.Application/"]
COPY ["CareGuard.Infrastructure/CareGuard.Infrastructure.csproj", "CareGuard.Infrastructure/"]
COPY ["CareGuard.Domain/CareGuard.Domain.csproj", "CareGuard.Domain/"]

RUN dotnet restore "CareGuard.API/CareGuard.API.csproj"

COPY . .

WORKDIR /app/CareGuard.API

RUN dotnet publish "CareGuard.API.csproj" -c Release -o /app/output 


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/output .

ENTRYPOINT ["dotnet", "CareGuard.API.dll"]