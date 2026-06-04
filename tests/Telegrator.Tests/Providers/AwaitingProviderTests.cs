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

using FluentAssertions;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;
using Telegrator.Providers;
using Xunit;

namespace Telegrator.Tests.Providers;

public class AwaitingProviderTests
{
    [Fact]
    public void TryGetDescriptorList_ShouldReturnFalse_WhenNoHandlersRegistered()
    {
        AwaitingProvider provider = new AwaitingProvider(new TelegratorOptions());

        bool found = provider.TryGetDescriptorList(UpdateType.Message, out HandlerDescriptorList? list);

        found.Should().BeFalse();
        list.Should().BeNull();
    }

    [Fact]
    public void TryGetDescriptorList_ShouldReturnTrue_WhenHandlerRegistered()
    {
        AwaitingProvider provider = new AwaitingProvider(new TelegratorOptions());
        ClassHandlerDescriptor handler = CreateTestHandlerDescriptor();

        using (provider.UseHandler(handler))
        {
            // AwaitingProvider is designed to register handlers dynamically and does not ties to a specific update type, so we can check for `UpdateType.Unknown` here.
            bool found = provider.TryGetDescriptorList(UpdateType.Unknown, out HandlerDescriptorList? list);
            found.Should().BeTrue();
            list.Should().NotBeNull();
            list!.Count.Should().Be(1);
        }
    }

    [Fact]
    public void UseHandler_ShouldAutoRemoveOnDispose()
    {
        AwaitingProvider provider = new AwaitingProvider(new TelegratorOptions());
        ClassHandlerDescriptor handler = CreateTestHandlerDescriptor();

        using (provider.UseHandler(handler))
        { }

        bool found = provider.TryGetDescriptorList(UpdateType.Message, out HandlerDescriptorList? list);
        found.Should().BeFalse();
        list.Should().BeNull();
    }

    [Fact]
    public void UseHandler_ShouldThrow_WhenDescriptorHasNoSingletonInstance()
    {
        AwaitingProvider provider = new AwaitingProvider(new TelegratorOptions());
        ClassHandlerDescriptor descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));

        System.Action act = () => provider.UseHandler(descriptor);

        act.Should().Throw<System.Exception>().WithMessage("*singleton instance*");
    }

    private static ClassHandlerDescriptor CreateTestHandlerDescriptor()
    {
        ClassHandlerDescriptor descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        descriptor.SetInstance(new TestUpdateHandler());
        return descriptor;
    }
}
