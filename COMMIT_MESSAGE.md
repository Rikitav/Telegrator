fix: resolve critical concurrency bugs, async anti-patterns and thread-safety issues

## Core Framework (Telegrator)
- **fix(router):** remove fire-and-forget in `UpdateRouter.HandleUpdateAsync` — now properly awaits internal pipeline
- **fix(router):** remove `.Result` deadlock in `DescribeHandler` — method is now fully `async` using `IAsyncEnumerable`
- **fix(descriptor):** replace `ManualResetEventSlim` with `TaskCompletionSource` in `DescribedHandlerDescriptor.AwaitResult` to eliminate thread-pool starvation and race condition
- **fix(pool):** move `ExecutionLimiter.Release` into `finally` block to prevent semaphore leaks on exceptions
- **fix(handler):** remove `catch (OperationCanceledException)` swallowing in `UpdateHandlerBase.Execute` — cancellation now correctly bubbles up as `null` result, stopping routing
- **fix(branching):** fix `BranchingUpdateHandler` crash on inherited `object` methods (`GetHashCode`, `ToString`, etc.) by utilizing `BranchesBindingFlags` and filtering by return type
- **fix(manager):** invert broken `HandlersManagerBase.IsEmpty` logic (was returning `true` when non-empty)
- **fix(client):** defer `GetMe()` call from `TelegratorClient` / `TelegratorWClient` constructors into `StartReceivingAsync`, removing synchronous blocking in ctors
- **fix(builder):** use `Interlocked.Increment` for `HandlerServiceKeyIndex` to prevent duplicate service keys under concurrent builds
- **fix(filters):** correct non-short-circuiting bitwise OR in `CompletedFiltersList`
- **fix(doc):** remove duplicate XML param tag in `UpdateRouter.DescribeHandler`

## Hosting (Telegrator.Hosting / .Web / .WideBot)
- **fix(hosting):** remove `.Result` / `.GetAwaiter().GetResult()` from `HostedTelegramBotInfo`, `HostedUpdateReceiver`, `HostedWideBotUpdateReceiver`, `TelegratorWClient.StartReceiving`
- **fix(hosting):** `HostedTelegramBotInfo.User` is now initialized asynchronously by receivers instead of blocking in constructor
- **fix(webhooker):** convert `async void StartInternal` to `async Task` and properly `await` it in `StartAsync`
- **fix(webhooker):** rewrite `ReceiveUpdate` as fully async `RequestDelegate`, removing all `.GetAwaiter().GetResult()` calls
- **fix(webhooker):** `StopAsync` now properly `await`s `DeleteWebhook`
- **fix(commands):** introduce `SetBotCommandsAsync` extension; mark sync `SetBotCommands` as `[Obsolete]` to discourage blocking calls
- **fix(interface):** add `BotInfo` property to `IUpdateRouter` for external access

## Localization (Telegrator.Localized)
- **fix(localization):** implement `IPostProcessor` in `LocalizedAspect` to restore `CurrentCulture` / `CurrentUICulture` after handler execution, preventing async context leaks
- **fix(localization):** replace throwing `NotImplementedException` in `DefaultStringLocalizer` with fail-soft behavior (returns key)

## Redis Storage (Telegrator.RedisStateStorage)
- **fix(redis):** honor `CancellationToken` in all `IStateStorage` methods by adding `ThrowIfCancellationRequested()`

## Testing (Telegrator.Tests)
- **fix(tests):** correct compile-time type mismatch in `TestServerExtensions` (`ServiceDescriptor` vs `IHostedService`)

## Misc
- **fix(strings):** correct off-by-one error in `StringExtensions.SliceBy`
- **fix(state):** fix typo in `DefaultStateStorage.GetAsync` parameter name (`ccancellationTokent` → `cancellationToken`)
- **fix(collections):** add thread-safety locks to `HandlerDescriptorList` read operations (`Count`, indexer, `ContainsKey`, `GetEnumerator`)
- **fix(collections):** reset internal `count` counter in `HandlerDescriptorList.Clear`
- **fix(collections):** `Freeze()` now protects `Remove` and `Clear` operations
- **fix(logging):** snapshot adapter list under lock in `TelegratorLogging.Log` to prevent `InvalidOperationException` during concurrent modification
- **chore:** rename `IPostProcessor.cs.cs` → `IPostProcessor.cs`
