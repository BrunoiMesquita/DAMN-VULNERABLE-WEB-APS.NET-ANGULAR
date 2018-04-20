#Stage 1
FROM microsoft/aspnetcore-build as builder
WORKDIR /api

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish /t:clean --output /app/ --configuration Release


#Stage 2
FROM microsoft/aspnetcore:2.0.6-jessie
 
WORKDIR /app
COPY --from=builder /app .
 
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
 
ENTRYPOINT ["dotnet", "net.mongo.dll"]