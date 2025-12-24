FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia il file .csproj e fai restore
COPY PhonebookAPI/PhonebookAPI.csproj PhonebookAPI/
WORKDIR /src/PhonebookAPI
RUN dotnet restore

# Copia tutto il resto del codice
WORKDIR /src
COPY PhonebookAPI/ PhonebookAPI/

# Build
WORKDIR /src/PhonebookAPI
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PhonebookAPI.dll"]