docker tag stack_api:latest damnstack/stack:stack_api.18.9.7
docker tag stack_nginx:latest damnstack/stack:stack_nginx.18.9.7

docker push damnstack/stack:stack_api.18.9.7
docker push damnstack/stack:stack_nginx.18.9.7

