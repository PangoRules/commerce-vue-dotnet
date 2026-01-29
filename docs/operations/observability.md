# Observability

## Current State: Minimal

The project currently has basic logging but lacks comprehensive observability features.

### What Exists

| Feature          | Status     | Notes                        |
| ---------------- | ---------- | ---------------------------- |
| Backend logging  | ✓ Basic    | Microsoft.Extensions.Logging |
| Frontend console | ✓ Basic    | console.log for development  |
| Docker logs      | ✓ Built-in | docker compose logs          |
| Metrics          | ✗ None     | -                            |
| Tracing          | ✗ None     | -                            |
| Alerting         | ✗ None     | -                            |

### Current Logging Configuration

Backend (`appsettings.json`):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Viewing Logs

```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f backend

# Last N lines
docker compose logs --tail 100 backend
```

## TODO: Logging Infrastructure

### Structured Logging

- [ ] Serilog for .NET with structured output
- [ ] JSON log format for parsing
- [ ] Correlation IDs across requests
- [ ] Request/response logging middleware

### Log Aggregation

- [ ] ELK Stack (Elasticsearch, Logstash, Kibana)
- [ ] Or: Loki + Grafana
- [ ] Or: Cloud logging (CloudWatch, Stackdriver)
- [ ] Log retention policies

### Frontend Logging

- [ ] Error tracking (Sentry, Bugsnag)
- [ ] Performance monitoring
- [ ] User session recording (optional)

## TODO: Metrics

### Application Metrics

- [ ] Prometheus metrics endpoint
- [ ] Request rate, latency, errors (RED)
- [ ] Database query performance
- [ ] Cache hit rates

### Infrastructure Metrics

- [ ] Container resource usage
- [ ] Database connections
- [ ] Storage utilization

### Dashboards

- [ ] Grafana dashboards
- [ ] Service health overview
- [ ] Business metrics (orders, products viewed)

## TODO: Distributed Tracing

### Implementation

- [ ] OpenTelemetry instrumentation
- [ ] Jaeger or Zipkin for trace collection
- [ ] Trace context propagation
- [ ] Sampling strategy

### What to Trace

- [ ] HTTP requests
- [ ] Database queries
- [ ] External API calls
- [ ] MinIO operations

## TODO: Alerting

### Alert Categories

- [ ] Error rate thresholds
- [ ] Latency degradation
- [ ] Resource exhaustion
- [ ] Health check failures

### Notification Channels

- [ ] Slack/Teams integration
- [ ] PagerDuty/OpsGenie
- [ ] Email alerts

## Recommended Stack

### Development

```
Logging: Console + Docker logs
Metrics: None (manual monitoring)
Tracing: None
```

### Production (Proposed)

```
Logging: Serilog → Loki → Grafana
Metrics: Prometheus → Grafana
Tracing: OpenTelemetry → Jaeger
Alerting: Grafana Alerting → Slack
```

## Implementation Priority

1. **High**: Structured logging with correlation IDs
2. **High**: Error tracking for frontend (Sentry)
3. **Medium**: Prometheus metrics endpoint
4. **Medium**: Grafana dashboards
5. **Low**: Distributed tracing
6. **Low**: Advanced alerting

## Quick Win: Health Endpoints

TODO: Currently we have health endpoints for the api and the database. We'll also need to check MinIO so we'll need a health endpoint for that.
