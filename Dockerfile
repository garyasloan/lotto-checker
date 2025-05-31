# ---------- Build backend (C# API) ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-backend
WORKDIR /src

COPY Api/Api.csproj ./Api/
RUN dotnet restore ./Api/Api.csproj

COPY Api/. ./Api/

COPY Api/appsettings.json ./Api/
#COPY Api/appsettings.Development.json ./Api/

WORKDIR /src/Api
RUN dotnet publish -c Release -o /app/publish

# ---------- Build frontend (React) ----------
FROM node:20 AS build-frontend
WORKDIR /app

COPY client/ ./client/
WORKDIR /app/client
RUN npm install
RUN npm run build

# ---------- Final image ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build-backend /app/publish .

COPY Api/appsettings.json ./
#COPY Api/appsettings.Development.json ./

COPY --from=build-frontend /app/client/dist ./wwwroot

# COPY --from=build-frontend /app/client/dist ./client/dist

# Serve static files if your API uses StaticFiles middleware
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80
ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENTRYPOINT ["dotnet", "Api.dll"]
