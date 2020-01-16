FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["frznUpload.Web/frznUpload.Web.csproj", "frznUpload.Web/"]
RUN dotnet restore "frznUpload.Web/frznUpload.Web.csproj"
COPY . .
WORKDIR "/src/frznUpload.Web"
RUN dotnet build "frznUpload.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "frznUpload.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "frznUpload.Web.dll"]