docker tag glubfish_api:latest dahln/glubfish:glubfish_api.18.7.9.1
docker tag glubfish_nginx:latest dahln/glubfish:glubfish_nginx.18.7.9.1

docker push dahln/glubfish:glubfish_api.18.7.9.1
docker push dahln/glubfish:glubfish_nginx.18.7.9.1

