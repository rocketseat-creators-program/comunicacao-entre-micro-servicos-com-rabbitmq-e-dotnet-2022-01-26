#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Inventory/Inventory.csproj", "src/Inventory/"]
RUN dotnet restore "src/Inventory/Inventory.csproj"
COPY . .
WORKDIR "/src/src/Inventory"
RUN dotnet build "Inventory.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Inventory.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.dll"]