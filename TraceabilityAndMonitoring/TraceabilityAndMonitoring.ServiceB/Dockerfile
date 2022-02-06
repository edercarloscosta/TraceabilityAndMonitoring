FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TraceabilityAndMonitoring.ServiceB/TraceabilityAndMonitoring.ServiceB.csproj", "TraceabilityAndMonitoring.ServiceB/"]
COPY ["TraceabilityAndMonitoring.Common/TraceabilityAndMonitoring.Common.csproj", "TraceabilityAndMonitoring.Common/"]
RUN dotnet restore "TraceabilityAndMonitoring.ServiceB/TraceabilityAndMonitoring.ServiceB.csproj"
COPY . .
WORKDIR "/src/TraceabilityAndMonitoring.ServiceB"
RUN dotnet build "TraceabilityAndMonitoring.ServiceB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraceabilityAndMonitoring.ServiceB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENV ASPNETCORE_URLS http://+:5002
ENTRYPOINT ["dotnet", "TraceabilityAndMonitoring.ServiceB.dll"]
