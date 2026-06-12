---
title: "OpenTelemetry Integration"
description: "Distributed tracing and metrics for Telegrator using System.Diagnostics.ActivitySource and Meter."
---

# Telegrator.OpenTelemetry

`Telegrator.OpenTelemetry` adds OpenTelemetry-compatible instrumentation on top of Telegrator's core services. It uses only `System.Diagnostics.ActivitySource` and `System.Diagnostics.Metrics.Meter`, so it has no hard dependency on the OpenTelemetry SDK — you can export traces and metrics with any exporter you choose.

## Installation

```shell
dotnet add package Telegrator.OpenTelemetry
```

## Registration

Register the instrumentation after `AddTelegrator()`:

```csharp
using Microsoft.Extensions.Hosting;
using Telegrator.Hosting;
using Telegrator.OpenTelemetry;

var builder = Host.CreateApplicationBuilder(args);

builder.AddTelegrator()
    .Handlers.CollectHandlers();

builder.Services.AddTelegratorOpenTelemetry();

builder.WithPolling();
var host = builder.Build();
host.UseTelegrator();
host.Run();
```

The extension decorates the registered services using Scrutor:

- `IUpdateHandlersPool` → `OpenTelemetryHandlersPool`
- `IAwaitingProvider` → `OpenTelemetryAwaitingProvider`
- `IStateStorage` → `OpenTelemetryStateStorage`

## Activity Source

The source name is `Telegrator`. Spans are created for:

- `HandleUpdate` — server span per incoming update
  - Tags: `telegrator.update.id`, `telegrator.update.type`
- `ExecuteHandler` — internal span per handler execution
  - Tags: `telegrator.handler.type`
- `AwaitUpdate` / `UseHandler` — awaiting handler registration
  - Tags: `telegrator.await.method`, `telegrator.await.handler`
- `StateOperation` — state storage calls
  - Tags: `telegrator.state.operation`

On exceptions, the state storage decorator sets `ActivityStatusCode.Error` and adds `exception.message` and `exception.stacktrace` tags.

## Metrics

The meter name is `Telegrator`. Instruments:

- `telegrator.updates.received` — counter of incoming updates
- `telegrator.handlers.executed` — counter of handler executions
- `telegrator.handlers.errors` — counter of handler failures
- `telegrator.handlers.duration` — histogram of handler execution duration in milliseconds
- `telegrator.awaiting.active` — up-down counter of active awaiting handlers
- `telegrator.state.operations` — counter of state storage operations

## Wiring an Exporter

Because the package only emits `ActivitySource`/`Meter` data, you need to reference an exporter or the OpenTelemetry SDK to see traces and metrics. A typical ASP.NET Core setup looks like this:

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

For production, replace `AddConsoleExporter()` with OTLP, Jaeger, Prometheus, or another supported exporter.

## Notes

- Core Telegrator already creates some spans internally. The OpenTelemetry package decorates the public service boundaries so you get consistent metrics even when core instrumentation changes.
- The package uses `Scrutor` for decoration. It is included as a transitive package reference.
