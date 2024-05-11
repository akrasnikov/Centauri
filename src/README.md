
## Docker-Compose Commands

```
docker-compose up  -d --build

```


Your API should be available at
- `https://localhost:8010/swagger/index.html` - order-webapi
- `http://localhost:8020/swagger/index.html` - moq-webapi

on server
- http://104.131.189.170/swagger/index.html - order-webapi
- http://104.131.189.170:8020/swagger/index.html - moq-webap
- http://104.131.189.170:8030/swagger/index.html - moq-webap
- http://104.131.189.170:8040/swagger/index.html - moq-webap

- http://104.131.189.170:30091/  - grafana  user & password = grafana
- http://104.131.189.170:30090/ - prometheus

- elasticsearch & kibana
- http://143.198.228.130:5601/app/discover#/?_g=(filters:!(),refreshInterval:(pause:!t,value:60000),time:(from:now%2Fw,to:now%2Fw))&_a=(columns:!(message,fields.CorrelationId),filters:!(),index:c37a7dde-fe6f-42f0-804d-9b13647cc83f,interval:auto,query:(language:kuery,query:''),sort:!(!('@timestamp',desc)))
- http://104.131.189.170:16686/ - jaeger

## Roadmap
 

1. deploy to server <br />
1.1 create docker-compose -ok <br />
1.2 deploy prometeus & grafana - ok <br />
1.3 deploy elasticsearch for logging <br />
1.4 configure nginx - ok <br />

2. create moq-webapi - ok <br />
3. retry with polly for agregator-webapi - ok <br />
4. configure dashboard for grafana & metrics - ok <br />
5. configure tracing  <br />
6. testing <br />
7. fixbug <br />
8. configure rate-limit for moq-webapi <br />
9. create integration test  using console application <br />

logic <br /> 
1. create order -> api -> POST/orders/order - ok <br />
2. run multiple tasks with batch using Hangfire in a background job - ok <br />
3. save progress in a distributed cache using Redis while running multiple tasks - ok  <br />
4. send metrics to prometeus - ok <br />
5. send traceId <br />
6. progress tracking using a Web API endpoint 'GET order/{id}' - ok  <br />
7. progress tracking using SignalR - need test <br />
