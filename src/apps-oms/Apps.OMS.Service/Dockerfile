FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["apps-oms/Apps.OMS.Service/Apps.OMS.Service.csproj", "apps-oms/Apps.OMS.Service/"]
COPY ["apps-filesystem/Apps.FileSystem.Export/Apps.FileSystem.Export.csproj", "apps-filesystem/Apps.FileSystem.Export/"]
COPY ["apps-common/Apps.Base.Common/Apps.Base.Common.csproj", "apps-common/Apps.Base.Common/"]
COPY ["apps-basic/Apps.Basic.Export/Apps.Basic.Export.csproj", "apps-basic/Apps.Basic.Export/"]
COPY ["apps-oms/Apps.OMS.Export/Apps.OMS.Export.csproj", "apps-oms/Apps.OMS.Export/"]
COPY ["apps-oms/Apps.OMS.Data/Apps.OMS.Data.csproj", "apps-oms/Apps.OMS.Data/"]
RUN dotnet restore "apps-oms/Apps.OMS.Service/Apps.OMS.Service.csproj"
COPY . .
WORKDIR "/src/apps-oms/Apps.OMS.Service"
RUN dotnet build "Apps.OMS.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Apps.OMS.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Apps.OMS.Service.dll"]