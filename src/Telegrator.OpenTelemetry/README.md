# Telegrator.OpenTelemetry

**Telegrator.OpenTelemetry** adds OpenTelemetry-compatible distributed tracing and metrics to the Telegrator Telegram bot framework. It uses only `System.Diagnostics.ActivitySource` and `System.Diagnostics.Metrics.Meter`, so it has no hard dependency on the OpenTelemetry SDK and works with any exporter you choose.

---

## Features

- Automatic instrumentation of core Telegrator services via decoration
- Server span per incoming Telegram update
- Internal spans for handler execution, awaiting handlers, and state storage operations
- Runtime metrics: updates received, handler executions/duration/errors, active awaiters, state operations
- Works with console, OTLP, Jaeger, Prometheus, or any other OpenTelemetry exporter
- Targets `netstandard2.0` — compatible with all Telegrator projects

---

## Requirements

- .NET 10.0 or later for the host application
- [Telegrator](https://github.com/Rikitav/Telegrator)
- An OpenTelemetry exporter or the OpenTelemetry SDK to collect traces and metrics

---

## Installation

```shell
dotnet add package Telegrator.OpenTelemetry
```

---

## Quick Start

Register the instrumentation after `AddTelegrator()`:

```csharp
using Microsoft.Extensions.Hosting;
using Telegrator.Hosting;
using Telegrator.OpenTelemetry;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddTelegrator()
    .Handlers.CollectHandlers();

builder.Services.AddTelegratorOpenTelemetry();

builder.WithPolling();

IHost host = builder.Build();
host.UseTelegrator();
host.Run();
```

The extension decorates the registered services using Scrutor:

- `IUpdateHandlersPool` → `OpenTelemetryHandlersPool`
- `IAwaitingProvider` → `OpenTelemetryAwaitingProvider`
- `IStateStorage` → `OpenTelemetryStateStorage`

---

## Activity Source

The source name is `Telegrator`. Spans are created for:

| Span name | Kind | Tags |
|-----------|------|------|
| `HandleUpdate` | Server | `telegrator.update.id`, `telegrator.update.type` |
| `ExecuteHandler` | Internal | `telegrator.handler.type` |
| `AwaitUpdate` / `UseHandler` | Internal | `telegrator.await.method`, `telegrator.await.handler` |
| `StateOperation` | Internal | `telegrator.state.operation` |

On exceptions, the state storage decorator sets `ActivityStatusCode.Error` and adds `exception.message` and `exception.stacktrace` tags.

---

## Metrics

The meter name is `Telegrator`. Instruments:

- `telegrator.updates.received` — counter of incoming updates
- `telegrator.handlers.executed` — counter of handler executions
- `telegrator.handlers.errors` — counter of handler execution failures
- `telegrator.handlers.duration` — histogram of handler execution duration in milliseconds
- `telegrator.awaiting.active` — up-down counter of active awaiting handlers
- `telegrator.state.operations` — counter of state storage operations

---

## Exporting Traces and Metrics

The package itself only emits telemetry. To see it, wire up an exporter, for example with the OpenTelemetry SDK in an ASP.NET Core application:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSource("Telegrator")
               .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddMeter("Telegrator")
               .AddConsoleExporter();
    });
```

For production, replace `AddConsoleExporter()` with `AddOtlpExporter()`, `AddJaegerExporter()`, or a Prometheus exporter.

---

## Notes

- Core Telegrator already creates some spans internally. The OpenTelemetry package decorates public service boundaries so you get consistent metrics even when core instrumentation changes.
- The package depends on `Scrutor` for service decoration; it is included as a transitive package reference.

---

## Documentation

- [Telegrator Main Repository](https://github.com/Rikitav/Telegrator)
- [OpenTelemetry Overview](https://github.com/Rikitav/Telegrator/wiki/OpenTelemetry)

---

## License

MIT
