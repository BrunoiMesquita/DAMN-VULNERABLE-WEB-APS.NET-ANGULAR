docker tag cdp_api:latest dahln/private:cdp_api.18.5.21.1
docker tag cdp_nginx:latest dahln/private:cdp_nginx.18.5.21.1

docker push dahln/private:cdp_api.18.5.21.1
docker push dahln/private:cdp_nginx.18.5.21.1

