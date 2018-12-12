docker tag stack_app:latest damnstack/stack:stack_app.4.2.1
docker tag stack_nginx:latest damnstack/stack:stack_nginx.4.2.1
docker push damnstack/stack:stack_app.4.2.1
docker push damnstack/stack:stack_nginx.4.2.1

