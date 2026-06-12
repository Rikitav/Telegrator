/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Telegram.Bot.Types.Enums;

namespace Telegrator.Annotations;

/// <summary>
/// Attribute that says if this handler can await some of await types, that is not listed by its handler base.
/// Used for automatic collecting allowed to receiving <see cref="UpdateType"/>'s.
/// If you don't use it, you won't be able to await the updates inside handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class MightAwaitAttribute : Attribute
{
    private readonly UpdateType[] _updateTypes;

    /// <summary>
    /// Update types that may be awaited
    /// </summary>
    public UpdateType[] UpdateTypes => _updateTypes;

    /// <summary>
    /// main ctor of <see cref="MightAwaitAttribute"/>
    /// </summary>
    /// <param name="updateTypes"></param>
    public MightAwaitAttribute(params UpdateType[] updateTypes)
        => _updateTypes = updateTypes;
}
