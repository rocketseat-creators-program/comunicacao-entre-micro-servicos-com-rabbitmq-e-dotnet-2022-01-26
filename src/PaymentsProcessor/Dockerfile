#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/PaymentsProcessor/PaymentsProcessor.csproj", "src/PaymentsProcessor/"]
COPY ["src/ExpertStore.SeedWork/ExpertStore.SeedWork.csproj", "src/ExpertStore.SeedWork/"]
RUN dotnet restore "src/PaymentsProcessor/PaymentsProcessor.csproj"
COPY . .
WORKDIR "/src/src/PaymentsProcessor"
RUN dotnet build "PaymentsProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentsProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentsProcessor.dll"]
