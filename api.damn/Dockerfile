#Stage 1
FROM microsoft/dotnet:2.2-sdk as builder

RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install nodejs -y

COPY ./api.damn ./api.damn
COPY ./data.damn ./data.damn
COPY ./logic.damn ./logic.damn
COPY ./model.damn ./model.damn
WORKDIR /api.damn
RUN dotnet restore
RUN dotnet publish /t:clean --output /api.damn/ --configuration Release


#Stage 2
FROM microsoft/dotnet:2.2-aspnetcore-runtime
 
WORKDIR /api.damn
COPY --from=builder /api.damn .
 
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
 
ENTRYPOINT ["dotnet", "api.damn.dll"]