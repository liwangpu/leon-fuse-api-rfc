FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY ["apps-legacy/ApiServer/ApiServer.csproj", "apps-legacy/ApiServer/"]
COPY ["apps-legacy/BambooCore/BambooCore.csproj", "apps-legacy/BambooCore/"]
COPY ["apps-legacy/BambooCommon/BambooCommon.csproj", "apps-legacy/BambooCommon/"]
COPY ["apps-legacy/ApiModel/ApiModel.csproj", "apps-legacy/ApiModel/"]
RUN dotnet restore "apps-legacy/ApiServer/ApiServer.csproj"
COPY . .
WORKDIR "/src/apps-legacy/ApiServer"
RUN dotnet build "ApiServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ApiServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ApiServer.dll"]