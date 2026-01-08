FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY PaymentsApi/*.csproj ./PaymentsApi/
RUN dotnet restore PaymentsApi/PaymentsApi.csproj
COPY . .
RUN dotnet publish PaymentsApi/PaymentsApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentsApi.dll"]
