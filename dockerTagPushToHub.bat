docker tag glubfish_api:latest dahln/glubfish:glubfish_api.18.5.22.3
docker tag glubfish_nginx:latest dahln/glubfish:glubfish_nginx.18.5.22.3

docker push dahln/glubfish:glubfish_api.18.5.22.3
docker push dahln/glubfish:glubfish_nginx.18.5.22.3

