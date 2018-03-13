#!/bin/bash
docker tag cdp_db:latest dahln/private:cdp_db.18.3.13.5
docker tag cdp_api:latest dahln/private:cdp_api.18.3.13.5
docker tag cdp_nginx:latest dahln/private:cdp_nginx.18.3.13.5

docker push dahln/private:cdp_db.18.3.13.5
docker push dahln/private:cdp_api.18.3.13.5
docker push dahln/private:cdp_nginx.18.3.13.5

