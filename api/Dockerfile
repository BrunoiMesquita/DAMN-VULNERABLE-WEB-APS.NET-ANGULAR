FROM microsoft/aspnetcore:2.0.5-jessie
 
WORKDIR /api
COPY bin/Debug/netcoreapp2.0/publish .
 
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
 
ENTRYPOINT ["dotnet", "net.mongo.dll"]