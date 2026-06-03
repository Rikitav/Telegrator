/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Attributes;

/// <summary>
/// Reactive way to implement a new <see cref="UpdateFilterAttribute{T}"/> of type <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class FilterAnnotation<T> : UpdateFilterAttribute<T>, IFilter<T>, INamedFilter where T : class
{
    /// <inheritdoc/>
    public virtual bool IsCollectible { get; } = false;

    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes { get; } = typeof(T).GetAllowedUpdateTypes();

    /// <inheritdoc/>
    public string Name => GetType().Name;

    /// <summary>
    /// Initializes new instance of <see cref="FilterAnnotation{T}"/>
    /// </summary>
    public FilterAnnotation() : base()
    {
        UpdateFilter = Filter<T>.If(CanPass);
        AnonymousFilter = AnonymousTypeFilter.Compile(UpdateFilter, GetFilterringTarget);
    }

    /// <inheritdoc/>
    public override T? GetFilterringTarget(Update update)
        => update.GetActualUpdateObject<T>();

    /// <inheritdoc/>
    public abstract bool CanPass(FilterExecutionContext<T> context);
}
