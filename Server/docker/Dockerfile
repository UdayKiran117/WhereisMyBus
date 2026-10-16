FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props global.json NuGet.Config ./
COPY src/ ./src/
RUN dotnet restore src/WhereIsMyBus.API/WhereIsMyBus.API.csproj
RUN dotnet publish src/WhereIsMyBus.API/WhereIsMyBus.API.csproj -c Release -o /out --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out ./
ENV ASPNETCORE_HTTP_PORTS=8080
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet","WhereIsMyBus.API.dll"]
