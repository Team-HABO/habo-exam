ARG APP_UID=1000

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ARG APP_UID=1000
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["rest.slnx", "./"]
COPY ["rest/rest.csproj", "rest/"]
RUN dotnet restore "rest/rest.csproj"
COPY ["rest/", "rest/"]
WORKDIR "/src/"
RUN dotnet build "rest/rest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "rest/rest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG APP_UID=1000
WORKDIR /app
USER root
COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .
COPY --from=build --chown=$APP_UID:$APP_UID /src/rest/movies.db ./movies.db
RUN chown -R $APP_UID:$APP_UID /app
USER $APP_UID
ENTRYPOINT ["dotnet", "rest.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS development
ARG APP_UID=1000
WORKDIR /src
RUN dotnet tool install -g dotnet-watch
ENV PATH="/root/.dotnet/tools:${PATH}"
COPY ["rest.slnx", "./"]
COPY ["rest/rest.csproj", "rest/"]
RUN dotnet restore "rest/rest.csproj"
ENTRYPOINT ["dotnet", "watch", "--", "run", "--urls", "http://0.0.0.0:8080"]