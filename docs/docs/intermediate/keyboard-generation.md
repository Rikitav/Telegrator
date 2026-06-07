# Keyboard Markup Generation

Telegrator ships with a **Roslyn source generator** that turns attributed `partial` members into fully-typed `InlineKeyboardMarkup` or `ReplyKeyboardMarkup` instances at compile time. This removes boilerplate when building static or mildly-parametrized keyboards, keeps the layout visually aligned with the final UI, and catches mistakes (wrong return type, unsupported buttons, etc.) before runtime.

## Prerequisites

- The containing project must reference `Telegrator.Analyzers` (transitive when you reference `Telegrator`).
- The declaring type must be `partial` so the generator can emit the implementation in a companion file.

## Supported Return Types

Only two return types are recognized:

| Return type | Generated markup |
|-------------|-----------------|
| `InlineKeyboardMarkup` | Inline keyboard |
| `ReplyKeyboardMarkup` | Reply keyboard |

Any other return type triggers **TLG201** (`Wrong return type`).

## Inline Keyboard Buttons

Apply one or more of the following attributes on a `partial` method or property. Each attribute list becomes one row; attributes inside the same list become buttons in that row.

| Attribute | Maps to |
|-----------|---------|
| `[CallbackButton]` | `InlineKeyboardButton.WithCallbackData` |
| `[GameButton]` | `InlineKeyboardButton.WithCallbackGame` |
| `[CopyTextButton]` | `InlineKeyboardButton.WithCopyText` |
| `[LoginRequestButton]` | `InlineKeyboardButton.WithLoginUrl` |
| `[PayRequestButton]` | `InlineKeyboardButton.WithPay` |
| `[UrlRedirectButton]` | `InlineKeyboardButton.WithUrl` |
| `[WebApp]` | `InlineKeyboardButton.WithWebApp` |
| `[SwitchQueryButton]` | `InlineKeyboardButton.WithSwitchInlineQuery` |
| `[QueryChosenButton]` | `InlineKeyboardButton.WithSwitchInlineQueryChosenChat` |
| `[QueryCurrentButton]` | `InlineKeyboardButton.WithSwitchInlineQueryCurrentChat` |

### Inline Example — Static Layout

```csharp
public partial class MyHandlers
{
    [CallbackButton("Yes", "yes")]
    [CallbackButton("No", "no")]
    private static partial InlineKeyboardMarkup ConfirmKeyboard();
}
```

The generator emits a `private static readonly` field and a bodyless accessor method that returns it:

```csharp
private static readonly InlineKeyboardMarkup ConfirmKeyboard_generatedMarkup = new InlineKeyboardMarkup(
[
    [InlineKeyboardButton.WithCallbackData("Yes", "yes")],
    [InlineKeyboardButton.WithCallbackData("No", "no")]
]);

private static partial InlineKeyboardMarkup ConfirmKeyboard()
    => ConfirmKeyboard_generatedMarkup;
```

### Inline Example — Parametrized Layout

If the `partial` method has parameters, the generator skips the cached field and creates the markup inline, substituting method arguments into string literals using interpolation:

```csharp
public partial class MyHandlers
{
    [CallbackButton("Cancel", "cancel"), CallbackButton("Apply", "apply:{value}")]
    private static partial InlineKeyboardMarkup ActionKeyboard(int value);
}
```

Generated code:

```csharp
private static partial InlineKeyboardMarkup ActionKeyboard(int value)
    => new InlineKeyboardMarkup(
    [
        [InlineKeyboardButton.WithCallbackData("Cancel", "cancel"),
         InlineKeyboardButton.WithCallbackData("Apply", $"apply:{value}")]
    ]);
```

> **Note:** Only string literals that contain `{paramName}` are converted to interpolated strings. Other argument types are passed through as-is.

## Reply Keyboard Buttons

Reply keyboards use the same mechanic with a different attribute set.

| Attribute | Maps to |
|-----------|---------|
| `[RequestChatButton]` | `KeyboardButton.WithRequestChat` |
| `[RequestContactButton]` | `KeyboardButton.WithRequestContact` |
| `[RequestLocationButton]` | `KeyboardButton.WithRequestLocation` |
| `[RequestPoolButton]` | `KeyboardButton.WithRequestPoll` |
| `[RequestUsersButton]` | `KeyboardButton.WithRequestUsers` |
| `[WebApp]` | `KeyboardButton.WithWebApp` |

### Reply Example

```csharp
public partial class MyHandlers
{
    [RequestContactButton("Share contact")]
    [RequestLocationButton("Share location")]
    private static partial ReplyKeyboardMarkup ShareKeyboard();
}
```

## Using Properties Instead of Methods

You can also declare a `partial` **get-only property** with the same attributes. It must not have a setter, initializer, expression body, or a bodied `get` accessor.

```csharp
public partial class MyHandlers
{
    [CallbackButton("Open", "open"), CallbackButton("Close", "close")]
    private static partial InlineKeyboardMarkup Menu { get; }
}
```

Properties are always generated inline (no cached field) and work the same way as parameterless methods.

## Row Layout Rules

- **Each `[…]` attribute list** (one pair of square brackets) produces **one row**.
- **Multiple attributes inside the same list** produce **multiple buttons in that row**.
- Separate rows = separate attribute lists.

```csharp
[CallbackButton("A1", "a1"), CallbackButton("A2", "a2")] // Row 1: two buttons
[CallbackButton("B1", "b1")]                              // Row 2: one button
private static partial InlineKeyboardMarkup Grid();
```

## Diagnostics Reference

The analyzer emits the following diagnostics when the declaration does not meet the generator’s requirements:

| ID | Severity | Trigger |
|----|----------|---------|
| **TLG201** | Error | Return type is not `InlineKeyboardMarkup` or `ReplyKeyboardMarkup` |
| **TLG202** | Error | Attribute is not supported for the chosen return type |
| **TLG203** | Error | Member is not declared as `partial` |
| **TLG204** | Error | Method has a body or expression body |
| **TLG206** | Error | Property has a setter, initializer, or expression body |
| **TLG207** | Error | Property `get` accessor has a body |

> ⚠️ **Known Issue:** `TLG201` is currently shared with `MightAwaitAnalyzer`. If you see an RS1019 warning during build, it is a benign ID collision and does not affect functionality. A future release will move keyboard diagnostics to a separate range (TLG301+).

## Limitations & Trade-Offs

1. **Compile-time only** — The layout is fully determined at compile time. You cannot hide, show, or reorder buttons based on runtime state (except via string interpolation of method arguments).
2. **No loops or conditions** — Because the layout is expressed through attributes, you cannot generate a dynamic number of rows or buttons from a collection.
3. **Verbosity at scale** — Keyboards with dozens of buttons become long attribute walls. For highly dynamic keyboards, constructing `InlineKeyboardMarkup` manually in code remains more readable.
4. **String-only interpolation** — Parameter substitution works only inside string literal arguments. You cannot pass a runtime object directly into an attribute argument.
5. **Partial requirement** — Both the member and the containing type must be `partial`. The generator automatically adds `partial` to enclosing classes/structs/records if it is missing.
6. **Nested types** — The generator supports nested classes, structs, and records, but not local functions or interface members.

## When to Use

| Scenario | Recommendation |
|----------|---------------|
| Static menus (settings, confirmation dialogs) | **Ideal** — minimal boilerplate, visual alignment with UI |
| Parametrized callbacks (`apply:{id}`) | **Good** — keeps markup declarative while allowing argument injection |
| Pagination, dynamic lists | **Avoid** — use imperative keyboard construction instead |
| Complex conditional layouts | **Avoid** — attributes cannot express branches or loops |

## Summary

`KeyboardMarkupGenerator` lets you declare Telegram keyboards as attributed `partial` members. It works for both inline and reply keyboards, supports parameter interpolation for dynamic callback data, and validates the declaration at compile time through dedicated diagnostics. Use it for static or lightly-parametrized layouts; fall back to manual `InlineKeyboardMarkup`/`ReplyKeyboardMarkup` construction when runtime flexibility is required.
