#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OceanBattle.WebAPI/OceanBattle.WebAPI.csproj", "OceanBattle.WebAPI/"]
COPY ["DataModel/DataModel/OceanBattle.DataModel.csproj", "DataModel/DataModel/"]
COPY ["OceanBattle.Rsa.Abstractions/OceanBattle.Rsa.Abstractions.csproj", "OceanBattle.Rsa.Abstractions/"]
COPY ["OceanBattle.Rsa/OceanBattle.Rsa.csproj", "OceanBattle.Rsa/"]
RUN dotnet restore "OceanBattle.WebAPI/OceanBattle.WebAPI.csproj"
COPY . .
WORKDIR "/src/OceanBattle.WebAPI"
RUN dotnet build "OceanBattle.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OceanBattle.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OceanBattle.WebAPI.dll"]