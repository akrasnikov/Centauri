#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Processing/Processing.Card.Host/Processing.Card.Host.csproj", "Processing/Processing.Card.Host/"]
RUN dotnet restore "./Processing/Processing.Card.Host/Processing.Card.Host.csproj"
COPY . .
WORKDIR "/src/Processing/Processing.Card.Host"
RUN dotnet build "./Processing.Card.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Processing.Card.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Processing.Card.Host.dll"]