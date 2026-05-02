FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MuseuDeBugs.Api/MuseuDeBugs.Api.csproj MuseuDeBugs.Api/
RUN dotnet restore MuseuDeBugs.Api/MuseuDeBugs.Api.csproj

COPY MuseuDeBugs.Api/ MuseuDeBugs.Api/
RUN dotnet publish MuseuDeBugs.Api/MuseuDeBugs.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000

CMD ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000} dotnet MuseuDeBugs.Api.dll
