#Stage 1
FROM microsoft/dotnet:2.1-sdk as builder

RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install nodejs -y

COPY ./app.stack ./app.stack
COPY ./data.stack ./data.stack
COPY ./logic.stack ./logic.stack
COPY ./model.stack ./model.stack
WORKDIR /app.stack
RUN dotnet restore
RUN dotnet publish /t:clean --output /app/ --configuration Release


#Stage 2
FROM microsoft/dotnet:2.1-aspnetcore-runtime
 
WORKDIR /app
COPY --from=builder /app .
 
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
 
ENTRYPOINT ["dotnet", "app.stack.dll"]