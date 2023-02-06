Trying to understand well known dotnet counters.

There is:
- a server to receive http requets
- a client to make the http requests
- dotnet monitor tool to gather well known dotnet counters
- k6 tool to generate load on the client
- prometheus to collect metrics
- grafana to visualize the metrics


# Docker setup
```
docker network create dotnet-monitoring-poc # All share this network
docker volume create dotnet-tmp # Client and dotnet-monitor mount this to enable dotnet monitor run as side car
```

# Server
```
cd server
docker build -t server . ; docker run --name server -it --rm --network dotnet-monitoring-poc --network-alias server --publish 5002:5002 server
```

To try open http://localhost:5002/WeatherForecast

# Client
```
cd client
docker build -t client . ; docker run --name client -it --rm --network dotnet-monitoring-poc --network-alias client --mount "source=dotnet-tmp,target=/tmp" --publish 5001:5001 client
```
To try open http://localhost:5001/WeatherForecast

# Dotnet monitor
```
docker run --name dotnet-monitor -it --rm --mount "source=dotnet-tmp,target=/tmp" --network dotnet-monitoring-poc --volume ${pwd}\dotnet-monitor-config.json:/tmp/dotnet-monitor-config.json -p 52323:52323 mcr.microsoft.com/dotnet/monitor collect --urls http://*:52323 --no-auth --configuration-file-path /tmp/dotnet-monitor-config.json
```
To view metrics, open http://localhost:52323/metrics

# Prometheus
```
cd prometheus
docker pull prom/prometheus:latest
docker run --rm -it --name prometheus --network dotnet-monitoring-poc --network-alias prometheus --publish 9090:9090 --volume ${pwd}\prometheus.yml:/etc/prometheus/prometheus.yml prom/prometheus
```
To view ui, open http://localhost:9090

# Grafana
```
docker pull grafana/grafana-oss:latest
docker run --rm -it --name grafana --network dotnet-monitoring-poc --network-alias grafana --publish 3000:3000 grafana/grafana-oss:latest
```
To view ui, open http://localhost:3000
Will have to create a new dashboard. Sample dashboard is in grafana folder.

# K6
```
docker pull grafana/k6
docker run --rm -it --network dotnet-monitoring-poc --volume ${pwd}\script.js:/script.js  grafana/k6 run  /script.js
```
