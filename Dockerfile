ARG MAIN_PROJECT="API"

FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build-env
ARG MAIN_PROJECT

WORKDIR /app

RUN apk add --update nodejs npm

# Copy csproj and restore as distinct layers
COPY . .

WORKDIR "/app/${MAIN_PROJECT}"

RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine
ARG MAIN_PROJECT

WORKDIR /app
COPY --from=build-env "/app/$MAIN_PROJECT/out" .
ENTRYPOINT ["/usr/bin/env"]
RUN ["dotnet", "${MAIN_PROJECT}.dll"]
