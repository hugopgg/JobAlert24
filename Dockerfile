FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -r linux-x64 

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/bin/Release/net8.0/linux-x64/publish .

COPY .env .env

EXPOSE 80

ENTRYPOINT ["dotnet", "JobAlert.dll"]
