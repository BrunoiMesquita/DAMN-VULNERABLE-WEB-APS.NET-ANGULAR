docker tag damn_api:latest dahln/damn-deploy:damn_api.5.0.0
docker tag damn_proxy:latest dahln/damn-deploy:damn_proxy.5.0.0
docker push dahln/damn-deploy:damn_api.5.0.0
docker push dahln/damn-deploy:damn_proxy.5.0.0

