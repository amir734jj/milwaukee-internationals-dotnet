FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env

WORKDIR /app

RUN apk add --update nodejs npm

# Copy csproj and restore as distinct layers
COPY . .

WORKDIR "/app/API"

RUN dotnet restore
RUN dotnet publish -c Release -o out

RUN npm install && npm run build
COPY client-build/* out/wwwwroot/

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

RUN apk add bash icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib && \
    apk add libgdiplus --repository https://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted

WORKDIR /app
COPY --from=build-env "/app/API/out" .
ENTRYPOINT ["dotnet", "API.dll"]
