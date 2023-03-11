#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["pulsa/pulsa.csproj", "pulsa/"]
RUN dotnet restore "pulsa/pulsa.csproj"
COPY . .
WORKDIR "/src/pulsa"
RUN dotnet build "pulsa.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pulsa.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:ASPNETCORE_ENVIRONMENT=Development

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pulsa.web.dll"]

#docker build . -t rifkisururi/belikouta.com.com:0.0.4