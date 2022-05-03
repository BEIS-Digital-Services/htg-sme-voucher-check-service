FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["VoucherCheckMicroService/VoucherCheckMicroService.csproj","VoucherCheckMicroService/"]
RUN dotnet restore "VoucherCheckMicroService/VoucherCheckMicroService.csproj"
COPY . .
WORKDIR "/src/VoucherCheckMicroService"
RUN dotnet publish "VoucherCheckMicroService.csproj" -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "VoucherCheckMicroService.dll"]