FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build-env

WORKDIR /app

RUN apk add --update nodejs npm

# Copy csproj and restore as distinct layers
COPY . .

WORKDIR "/app/API"

RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine

WORKDIR /app
COPY --from=build-env "/app/API/out" .
ENTRYPOINT ["dotnet", "API.dll"]
