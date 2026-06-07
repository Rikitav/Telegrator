# Keyboard Markup Generation

Telegrator ships with a **Roslyn source generator** that turns attributed `partial` members into fully-typed `InlineKeyboardMarkup` or `ReplyKeyboardMarkup` instances at compile time. This removes boilerplate when building static or mildly-parametrized keyboards, keeps the layout visually aligned with the final UI, and catches mistakes (wrong return type, unsupported buttons, etc.) before runtime.

## Prerequisites

- The containing project must reference `Telegrator.Analyzers` (transitive when you reference `Telegrator`).
- The declaring type must be `partial` so the generator can emit the implementation in a companion file.

## Supported Return Types

Only two return types are recognized:

<table>
<thead>
<tr><th>Return type</th><th>Generated markup</th></tr>
</thead>
<tbody>
<tr><td><code>InlineKeyboardMarkup</code></td><td>Inline keyboard</td></tr>
<tr><td><code>ReplyKeyboardMarkup</code></td><td>Reply keyboard</td></tr>
</tbody>
</table>

Any other return type triggers **TLG201** (`Wrong return type`).

## Inline Keyboard Buttons

Apply one or more of the following attributes on a `partial` method or property. Each attribute list becomes one row; attributes inside the same list become buttons in that row.

<table>
<thead>
<tr><th>Attribute</th><th>Maps to</th></tr>
</thead>
<tbody>
<tr><td><code>[CallbackButton]</code></td><td><code>InlineKeyboardButton.WithCallbackData</code></td></tr>
<tr><td><code>[GameButton]</code></td><td><code>InlineKeyboardButton.WithCallbackGame</code></td></tr>
<tr><td><code>[CopyTextButton]</code></td><td><code>InlineKeyboardButton.WithCopyText</code></td></tr>
<tr><td><code>[LoginRequestButton]</code></td><td><code>InlineKeyboardButton.WithLoginUrl</code></td></tr>
<tr><td><code>[PayRequestButton]</code></td><td><code>InlineKeyboardButton.WithPay</code></td></tr>
<tr><td><code>[UrlRedirectButton]</code></td><td><code>InlineKeyboardButton.WithUrl</code></td></tr>
<tr><td><code>[WebApp]</code></td><td><code>InlineKeyboardButton.WithWebApp</code></td></tr>
<tr><td><code>[SwitchQueryButton]</code></td><td><code>InlineKeyboardButton.WithSwitchInlineQuery</code></td></tr>
<tr><td><code>[QueryChosenButton]</code></td><td><code>InlineKeyboardButton.WithSwitchInlineQueryChosenChat</code></td></tr>
<tr><td><code>[QueryCurrentButton]</code></td><td><code>InlineKeyboardButton.WithSwitchInlineQueryCurrentChat</code></td></tr>
</tbody>
</table>

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

<table>
<thead>
<tr><th>Attribute</th><th>Maps to</th></tr>
</thead>
<tbody>
<tr><td><code>[RequestChatButton]</code></td><td><code>KeyboardButton.WithRequestChat</code></td></tr>
<tr><td><code>[RequestContactButton]</code></td><td><code>KeyboardButton.WithRequestContact</code></td></tr>
<tr><td><code>[RequestLocationButton]</code></td><td><code>KeyboardButton.WithRequestLocation</code></td></tr>
<tr><td><code>[RequestPoolButton]</code></td><td><code>KeyboardButton.WithRequestPoll</code></td></tr>
<tr><td><code>[RequestUsersButton]</code></td><td><code>KeyboardButton.WithRequestUsers</code></td></tr>
<tr><td><code>[WebApp]</code></td><td><code>KeyboardButton.WithWebApp</code></td></tr>
</tbody>
</table>

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

<table>
<thead>
<tr><th>ID</th><th>Severity</th><th>Trigger</th></tr>
</thead>
<tbody>
<tr><td><strong>TLG201</strong></td><td>Error</td><td>Return type is not <code>InlineKeyboardMarkup</code> or <code>ReplyKeyboardMarkup</code></td></tr>
<tr><td><strong>TLG202</strong></td><td>Error</td><td>Attribute is not supported for the chosen return type</td></tr>
<tr><td><strong>TLG203</strong></td><td>Error</td><td>Member is not declared as <code>partial</code></td></tr>
<tr><td><strong>TLG204</strong></td><td>Error</td><td>Method has a body or expression body</td></tr>
<tr><td><strong>TLG206</strong></td><td>Error</td><td>Property has a setter, initializer, or expression body</td></tr>
<tr><td><strong>TLG207</strong></td><td>Error</td><td>Property <code>get</code> accessor has a body</td></tr>
</tbody>
</table>

> ⚠️ **Known Issue:** `TLG201` is currently shared with `MightAwaitAnalyzer`. If you see an RS1019 warning during build, it is a benign ID collision and does not affect functionality. A future release will move keyboard diagnostics to a separate range (TLG301+).

## Limitations & Trade-Offs

1. **Compile-time only** — The layout is fully determined at compile time. You cannot hide, show, or reorder buttons based on runtime state (except via string interpolation of method arguments).
2. **No loops or conditions** — Because the layout is expressed through attributes, you cannot generate a dynamic number of rows or buttons from a collection.
3. **Verbosity at scale** — Keyboards with dozens of buttons become long attribute walls. For highly dynamic keyboards, constructing `InlineKeyboardMarkup` manually in code remains more readable.
4. **String-only interpolation** — Parameter substitution works only inside string literal arguments. You cannot pass a runtime object directly into an attribute argument.
5. **Partial requirement** — Both the member and the containing type must be `partial`. The generator automatically adds `partial` to enclosing classes/structs/records if it is missing.
6. **Nested types** — The generator supports nested classes, structs, and records, but not local functions or interface members.

## When to Use

<table>
<thead>
<tr><th>Scenario</th><th>Recommendation</th></tr>
</thead>
<tbody>
<tr><td>Static menus (settings, confirmation dialogs)</td><td><strong>Ideal</strong> — minimal boilerplate, visual alignment with UI</td></tr>
<tr><td>Parametrized callbacks (<code>apply:{id}</code>)</td><td><strong>Good</strong> — keeps markup declarative while allowing argument injection</td></tr>
<tr><td>Pagination, dynamic lists</td><td><strong>Avoid</strong> — use imperative keyboard construction instead</td></tr>
<tr><td>Complex conditional layouts</td><td><strong>Avoid</strong> — attributes cannot express branches or loops</td></tr>
</tbody>
</table>

## Summary

`KeyboardMarkupGenerator` lets you declare Telegram keyboards as attributed `partial` members. It works for both inline and reply keyboards, supports parameter interpolation for dynamic callback data, and validates the declaration at compile time through dedicated diagnostics. Use it for static or lightly-parametrized layouts; fall back to manual `InlineKeyboardMarkup`/`ReplyKeyboardMarkup` construction when runtime flexibility is required.
