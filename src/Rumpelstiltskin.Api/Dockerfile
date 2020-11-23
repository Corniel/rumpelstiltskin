FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Rumpelstiltskin.Api.csproj", ""]
RUN dotnet restore "./Rumpelstiltskin.Api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Rumpelstiltskin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rumpelstiltskin.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rumpelstiltskin.Api.dll"]
