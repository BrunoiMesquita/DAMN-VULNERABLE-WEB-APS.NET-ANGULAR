docker tag stack_app:latest damnstack/stack:stack_app.4.0.3
docker tag stack_nginx:latest damnstack/stack:stack_nginx.4.0.3
docker push damnstack/stack:stack_app.4.0.3
docker push damnstack/stack:stack_nginx.4.0.3

