FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Tanish.Domain/Tanish.Domain.csproj Tanish.Domain/
COPY Tanish.Application/Tanish.Application.csproj Tanish.Application/
COPY Tanish.Infrostructure/Tanish.Infrastructure.csproj Tanish.Infrostructure/
COPY Tanish.Api/Tanish.Api.csproj Tanish.Api/

RUN dotnet restore Tanish.Api/Tanish.Api.csproj

COPY . .
RUN dotnet publish Tanish.Api/Tanish.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Tanish.Api.dll"]