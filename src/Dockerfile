FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY CareerPoint.sln .
COPY src/CareerPoint.API/*.csproj ./src/CareerPoint.API/
COPY src/CareerPoint.Application/*.csproj ./src/CareerPoint.Application/
COPY src/CareerPoint.Infrastructure/*.csproj ./src/CareerPoint.Infrastructure/
COPY tests/CareerPoint.Application.Tests/*.csproj ./tests/CareerPoint.Application.Tests/

RUN dotnet restore CareerPoint.sln

COPY . ./

RUN dotnet publish ./src/CareerPoint.API/CareerPoint.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CareerPoint.Web.dll"]