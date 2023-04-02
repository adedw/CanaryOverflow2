FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CanaryOverflow2.WebAPI/CanaryOverflow2.WebAPI.csproj", "CanaryOverflow2.WebAPI/"]
RUN dotnet restore "CanaryOverflow2.WebAPI/CanaryOverflow2.WebAPI.csproj"
COPY . .
WORKDIR "/src/CanaryOverflow2.WebAPI"
RUN dotnet build "CanaryOverflow2.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CanaryOverflow2.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CanaryOverflow2.WebAPI.dll"]
