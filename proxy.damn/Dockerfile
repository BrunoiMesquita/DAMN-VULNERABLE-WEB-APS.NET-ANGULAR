#stage 1

# We label our stage as ‘builder’
FROM node:latest as builder

COPY ./web.damn ./web.damn
WORKDIR /web.damn
RUN npm install

## Build the angular app in production mode and store the artifacts in dist folder
RUN npm install typescript@'>=3.1.1 <=3.1.1'
RUN $(npm bin)/ng build

#stage 2
FROM nginx
COPY --from=builder /web.damn/dist /usr/share/nginx/html
COPY proxy.damn/ssl /usr/share/nginx/ssl
COPY proxy.damn/nginx.conf /etc/nginx/nginx.conf