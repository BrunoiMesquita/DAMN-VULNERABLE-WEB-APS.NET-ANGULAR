#!/bin/bash
docker-compose bundle
docker tag cdp_db dahln/private:cdp_db.18.3.12.4
docker tag cdp_api dahln/private:cdp_api.18.3.12.4
docker tag cdp_ng dahln/private:cdp_nginx.18.3.12.4

docker push dahln/private:cdp_db.18.3.12.4
docker push dahln/private:cdp_api.18.3.12.4
docker push dahln/private:cdp_nginx.18.3.12.4

