FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS Base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiGateway.csproj", "."]
RUN dotnet restore "ApiGateway.csproj"
COPY . .

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish --no-restore

FROM Base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]