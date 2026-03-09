FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/Catalog/BookStore.Catalog/BookStore.Catalog.csproj", "Services/Catalog/BookStore.Catalog/"]
COPY ["Aspire/Aspire.ServiceDefaults/Aspire.ServiceDefaults.csproj", "Aspire/Aspire.ServiceDefaults/"]
COPY ["BuildingBlocks/BuildingBlocks.Chassis/BuildingBlocks.Chassis.csproj", "BuildingBlocks/BuildingBlocks.Chassis/"]
COPY ["BuildingBlocks/BuildingBlocks.Constants/BuildingBlocks.Constants.csproj", "BuildingBlocks/BuildingBlocks.Constants/"]
COPY ["BuildingBlocks/BuildingBlocks.SharedKernel/BuildingBlocks.SharedKernel.csproj", "BuildingBlocks/BuildingBlocks.SharedKernel/"]

RUN dotnet restore "Services/Catalog/BookStore.Catalog/BookStore.Catalog.csproj"

COPY . .

RUN dotnet build "Services/Catalog/BookStore.Catalog/BookStore.Catalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Services/Catalog/BookStore.Catalog/BookStore.Catalog.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BookStore.Catalog.dll"]
