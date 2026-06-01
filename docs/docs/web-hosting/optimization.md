---
title: "Web Optimization"
description: "Fine-tuning performance for Webhook-based bots."
---

# Webhook Optimization

Running a bot on Webhooks in a web server environment has different performance characteristics than long-polling in a console app.

## Non-Blocking Processing

When an update arrives at the Webhook endpoint:
1. `Telegrator.Hosting.Web` reads the JSON.
2. It pushes the update into an internal `Channel<UpdateHandlerContext>`.
3. It immediately returns `200 OK` to Telegram.
4. An internal background worker picks up the update from the channel and executes the handlers.

This ensures that even if a handler is slow (e.g., waiting for AI response), the Webhook request is closed quickly, preventing Telegram from marking your bot as "down".

## Concurrency Control

You can control how many updates are processed simultaneously using `TelegratorOptions`:

```csharp
options.MaximumParallelWorkingHandlers = 100; // Handle 100 updates in parallel
```

## Deployment Tips

- **Azure Functions / AWS Lambda**: Telegrator can work here, but ensure you use the `TelegratorClient` manually if you don't have a persistent environment for Generic Host.
- **Reverse Proxy**: If using Nginx or Caddy, ensure they forward the `X-Forwarded-For` headers if you use IP filtering.
- **Port**: Telegram only supports a few ports for webhooks: 443, 80, 88, 8443.

## Limitations
- **Cold Starts**: If using Serverless (like Google Cloud Run), the first update might be slow due to container startup.
- **Local Debugging**: Requires a tool like `ngrok` or `devtunnel` to expose your local port to the internet.
