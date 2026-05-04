FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Lernzeit-Backend/Lernzeit-Backend.csproj", "Lernzeit-Backend/"]
RUN dotnet restore "Lernzeit-Backend/Lernzeit-Backend.csproj"
COPY . .
WORKDIR "/src/Lernzeit-Backend"
RUN dotnet build "./Lernzeit-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Lernzeit-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lernzeit-Backend.dll"]
