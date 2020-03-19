FROM mcr.microsoft.com/dotnet/core/sdk:2.2.402 AS build-env

LABEL maintainer="Rafael Karayannopoulos 'malkaviano'"

COPY . ./app

WORKDIR /app

RUN dotnet restore

RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

COPY --from=build-env /app/Hello/out /app

COPY --from=build-env /app/Hello/appsettings.json /app

WORKDIR /app

ENTRYPOINT ["dotnet", "Hello.dll"]
