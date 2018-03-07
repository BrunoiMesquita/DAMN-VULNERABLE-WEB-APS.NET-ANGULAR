#!/bin/bash
docker-compose bundle
docker tag cdp_db dahln/private:cdp_db.18.3.6.3
docker tag cdp_api dahln/private:cdp_api.18.3.6.3
docker tag cdp_ng dahln/private:cdp_ng.18.3.6.3
docker tag cdp_proxy dahln/private:cdp_proxy.18.3.6.3

docker push dahln/private:cdp_db.18.3.6.3
docker push dahln/private:cdp_api.18.3.6.3
docker push dahln/private:cdp_ng.18.3.6.3
docker push dahln/private:cdp_proxy.18.3.6.3

