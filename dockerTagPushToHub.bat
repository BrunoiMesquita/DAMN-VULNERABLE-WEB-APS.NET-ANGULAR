docker tag damnstack_api:latest damnstack/damnstack:damnstack_api.18.9.1
docker tag damnstack_nginx:latest damnstack/damnstack:damnstack_nginx.18.9.1

docker push damnstack/damnstack:damnstack_api.18.9.1
docker push damnstack/damnstack:damnstack_nginx.18.9.1

