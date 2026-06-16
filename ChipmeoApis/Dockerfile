FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ChipmeoApis.Core/*.csproj ./ChipmeoApis.Core/
COPY ChipmeoApis.Usecase/*.csproj ./ChipmeoApis.Usecase/
COPY ChipmeoApis.Infrastructure/*.csproj ./ChipmeoApis.Infrastructure/
COPY ChipmeoApis.Web/*.csproj ./ChipmeoApis.Web/

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore ChipmeoApis.Web/ChipmeoApis.Web.csproj

COPY . .

RUN dotnet publish ChipmeoApis.Web/ChipmeoApis.Web.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ChipmeoApis.Web.dll"]
