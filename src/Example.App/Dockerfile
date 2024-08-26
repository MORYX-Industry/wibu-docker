#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Build.targets", "."]
COPY ["NuGet.Config", "."]
COPY ["src/Example.App/Example.App.csproj", "src/Example.App/"]
COPY ["src/Example/Example.csproj", "src/Example/"]
COPY ["src/Example.ControlSystem/Example.ControlSystem.csproj", "src/Example.ControlSystem/"]
COPY ["src/Example.Orders/Example.Orders.csproj", "src/Example.Orders/"]
COPY ["src/Example.Products/Example.Products.csproj", "src/Example.Products/"]
COPY ["src/Example.Resources/Example.Resources.csproj", "src/Example.Resources/"]
RUN dotnet restore "./src/Example.App/Example.App.csproj"
COPY . .
WORKDIR "/src/src/Example.App"
RUN dotnet build "./Example.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Example.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY wibu/deb/usr/lib/x86_64-linux-gnu/libwibucm.so /usr/lib/x86_64-linux-gnu/libwibucm.so
COPY wibu/deb/usr/lib/x86_64-linux-gnu/libcpsrt.so /usr/lib/x86_64-linux-gnu/libcpsrt.so
ENTRYPOINT ["dotnet", "Example.App.dll"]