#stage 1

# We label our stage as ‘builder’
FROM node:latest as builder

RUN mkdir /app
WORKDIR /app
COPY . .
RUN npm install

## Build the angular app in production mode and store the artifacts in dist folder
RUN npm install typescript@'>=2.7.0 <2.8.0'
RUN $(npm bin)/ng build

#stage 2
FROM nginx
COPY --from=builder /app/dist /usr/share/nginx/html
COPY ssl /usr/share/nginx/ssl
COPY nginx.conf /etc/nginx/nginx.conf
