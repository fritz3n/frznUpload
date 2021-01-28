#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="$PATH:/root/.dotnet/tools"
COPY ["frznUpload.Web/frznUpload.Web.csproj", "frznUpload.Web/"]
COPY ["frznUpload.Shared/frznUpload.Shared.csproj", "frznUpload.Shared/"]
RUN dotnet restore "frznUpload.Web/frznUpload.Web.csproj"
COPY . .
WORKDIR "/src/frznUpload.Web"
RUN libman restore
RUN dotnet build "frznUpload.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "frznUpload.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "frznUpload.Web.dll"]