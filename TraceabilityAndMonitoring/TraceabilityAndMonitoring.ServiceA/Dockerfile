FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TraceabilityAndMonitoring.ServiceA/TraceabilityAndMonitoring.ServiceA.csproj", "TraceabilityAndMonitoring.ServiceA/"]
COPY ["TraceabilityAndMonitoring.Common/TraceabilityAndMonitoring.Common.csproj", "TraceabilityAndMonitoring.Common/"]
RUN dotnet restore "TraceabilityAndMonitoring.ServiceA/TraceabilityAndMonitoring.ServiceA.csproj"
COPY . .
WORKDIR "/src/TraceabilityAndMonitoring.ServiceA"
RUN dotnet build "TraceabilityAndMonitoring.ServiceA.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraceabilityAndMonitoring.ServiceA.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENV ASPNETCORE_URLS http://+:5004
ENTRYPOINT ["dotnet", "TraceabilityAndMonitoring.ServiceA.dll"]
