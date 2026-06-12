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

using Telegrator.Core.Attributes;

namespace Telegrator.Handlers.Diagnostics;

/// <summary>
/// Provides extension methods for <see cref="ReportInspector"/>
/// </summary>
public static partial class ReportInspectorExtensions
{
    /// <inheritdoc cref="ReportInspector.Whenever(string)"/>
    public static ReportInspector Whenever<TAttribute>(this ReportInspector inspector) where TAttribute : UpdateFilterAttributeBase
        => inspector.Whenever(nameof(TAttribute));

    /// <inheritdoc cref="ReportInspector.Except(string)"/>
    public static ReportInspector Except<TAttribute>(this ReportInspector inspector) where TAttribute : UpdateFilterAttributeBase
        => inspector.Except(nameof(TAttribute));
}
