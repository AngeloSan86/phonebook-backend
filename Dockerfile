FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PhonebookAPI/PhonebookAPI.csproj", "PhonebookAPI/"]
RUN dotnet restore "PhonebookAPI/PhonebookAPI.csproj"
COPY PhonebookAPI/ PhonebookAPI/
WORKDIR "/src/PhonebookAPI"
RUN dotnet build "PhonebookAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhonebookAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "PhonebookAPI.dll"]