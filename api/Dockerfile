FROM microsoft/aspnetcore:2.0.6-jessie
 
WORKDIR /api
COPY bin/Debug/netcoreapp2.0/publish .
 
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
 
ENTRYPOINT ["dotnet", "net.mongo.dll"]




# FROM microsoft/aspnetcore:2.0.6-jessie AS base
# WORKDIR /app
# EXPOSE 80

# FROM microsoft/aspnetcore-build:2.0 AS build
# WORKDIR /src
# COPY net.mongo.sln ./
# COPY net.mongo/net.mongo.csproj net.mongo/
# RUN dotnet restore -nowarn:msb3202,nu1503
# COPY . .
# WORKDIR /src/net.mongo
# RUN dotnet build -c Release -o /app

# FROM build AS publish
# RUN dotnet publish -c Release -o /app

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app .

# ENV ASPNETCORE_URLS http://+:5000
# EXPOSE 5000

# ENTRYPOINT ["dotnet", "net.mongo.dll"]
