#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ExpertStore.Ordering/ExpertStore.Ordering.csproj", "src/ExpertStore.Ordering/"]
COPY ["src/ExpertStore.SeedWork/ExpertStore.SeedWork.csproj", "src/ExpertStore.SeedWork/"]
RUN dotnet restore "src/ExpertStore.Ordering/ExpertStore.Ordering.csproj"
COPY . .
WORKDIR "/src/src/ExpertStore.Ordering"
RUN dotnet build "ExpertStore.Ordering.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExpertStore.Ordering.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpertStore.Ordering.dll"]
