---
title: "Distributed Concurrency"
description: "Handling state updates safely across multiple bot instances."
---

# Distributed Concurrency

In a distributed environment (e.g., 3 instances of your bot running in Kubernetes), two updates from the same user could arrive at different instances at the same time.

## The Problem
If Instance A and Instance B both read the current state, modify it, and write it back, one update might overwrite the other. This is known as a **Race Condition**.

## Redis Implementation
The `RedisStateStorage` uses atomic Redis operations for simple set/get. However, complex "Read-Modify-Write" cycles (like state transitions) still need care.

### 1. Atomic Transitions
Telegrator's `Advance()` and `Retreat()` methods on `StateMachine` are designed to be as safe as possible.

### 2. Manual Locking (Advanced)
If you have extremely sensitive state logic, consider using a Redis Distributed Lock (e.g., via [RedLock.net](https://github.com/samcook/RedLock.net)) inside your handler:

```csharp
var lockKey = $"lock:user:{userId}";
using (var redLock = await lockFactory.CreateLockAsync(lockKey, TimeSpan.FromSeconds(10)))
{
    if (redLock.IsAcquired)
    {
        // Safe Read-Modify-Write
    }
}
```

## Idempotency
A better approach is to design your handlers to be **idempotent**. This means that if a handler runs twice with the same update, the result is the same as running it once.

## Limitations
- **Network Latency**: Redis is fast, but network round-trips add up.
- **Consistency**: Telegrator favors "Availability" (AP) over "Consistency" (CP) in standard state storage to keep the bot responsive.
