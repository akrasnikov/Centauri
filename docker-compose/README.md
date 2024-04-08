
## Docker-Compose Commands

.NET WebAPI Boilerplate includes 3 Docker-Compose Files!
- WebAPI + PostgreSQL (default)

1) WebAPI + PostgreSQL (default)
```
docker-compose up  -d --build

```


Your API should be available at
- `https://localhost:8010/swagger/index.html` - order-webapi
- `http://localhost:8020/swagger/index.html` - moq-webapi

on server
- http://104.131.189.170/swagger/index.html - order-webapi
- http://104.131.189.170:8020/swagger/index.html - moq-webap

- http://104.131.189.170:30091/  - grafana  user & password = grafana
- http://104.131.189.170:30090/ - prometheus

## Roadmap
 

1. deploy to server
1.1 create docker-compose -ok
1.2 deploy prometeus & grafana - ok
1.3 deploy elasticsearch for logging
1.4 configure nginx - ok

2. create moq-webapi - ok
3. retry with polly for agregator-webapi - ok
4. configure dashboard for grafana & metrics - ok
5. configure tracing  
6. testing
7. fixbug
8. configure rate-limit for moq-webapi
9. create integration test  using console application 

logic 
1. create order -> webapi post: orders/order - ok
2. run multiple tasks with batch using Hangfire in a background job - ok
3. save progress in a distributed cache using Redis while running multiple tasks - ok 
4. send metrics to prometeus - ok 
5. send traceId
6. progress tracking using a Web API endpoint 'GET order/{id}' - ok 
7. progress tracking using SignalR - need test 
