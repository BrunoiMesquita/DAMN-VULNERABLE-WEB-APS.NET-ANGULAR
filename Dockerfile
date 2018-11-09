#Stage 1
FROM microsoft/dotnet:2.1-sdk as builder
WORKDIR /api

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish /t:clean --output /app/ --configuration Release


#Stage 2
FROM microsoft/dotnet:2.1-aspnetcore-runtime
 
WORKDIR /app
COPY --from=builder /app .
 
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
 
ENTRYPOINT ["dotnet", "app.stack.dll"]