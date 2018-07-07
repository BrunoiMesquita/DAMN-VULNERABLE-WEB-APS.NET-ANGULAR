#Stage 1
FROM microsoft/dotnet:2.1.301-sdk-alpine3.7 as builder
WORKDIR /api

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish /t:clean --output /app/ --configuration Release


#Stage 2
FROM microsoft/dotnet:2.1.1-aspnetcore-runtime-alpine3.7
 
WORKDIR /app
COPY --from=builder /app .
 
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
 
ENTRYPOINT ["dotnet", "glubfish.dll"]