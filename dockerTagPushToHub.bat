docker tag stack_app:latest damnstack/stack:stack_app.4.2.0
docker tag stack_nginx:latest damnstack/stack:stack_nginx.4.2.0
docker push damnstack/stack:stack_app.4.2.0
docker push damnstack/stack:stack_nginx.4.2.0

