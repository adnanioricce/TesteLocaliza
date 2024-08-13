FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5046

ENV ASPNETCORE_URLS=http://+:5046

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Localiza.Web/Localiza.Web.csproj", "Localiza.Web/"]
RUN dotnet restore "Localiza.Web/Localiza.Web.csproj"
COPY . .
WORKDIR "/src/Localiza.Web"
RUN dotnet build "Localiza.Web.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
# Install Node.js and npm
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs
RUN dotnet publish "Localiza.Web.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Localiza.Web.dll"]
