FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props ./
COPY src/HoangPhucSpa.Api/HoangPhucSpa.Api.csproj src/HoangPhucSpa.Api/
COPY src/HoangPhucSpa.Data/HoangPhucSpa.Data.csproj src/HoangPhucSpa.Data/
RUN dotnet restore src/HoangPhucSpa.Api/HoangPhucSpa.Api.csproj
COPY src/HoangPhucSpa.Api/ src/HoangPhucSpa.Api/
COPY src/HoangPhucSpa.Data/ src/HoangPhucSpa.Data/
RUN dotnet publish src/HoangPhucSpa.Api/HoangPhucSpa.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HoangPhucSpa.Api.dll"]
