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

public class HandlersProviderTests
{
    [Fact]
    public void IsEmpty_ShouldReturnTrue_ForEmptyCollection()
    {
        var collection = new HandlersCollection(new TelegratorOptions());
        var provider = new HandlersProvider(collection, new TelegratorOptions());

        provider.IsEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_ShouldReturnFalse_WhenHandlersExist()
    {
        var collection = new HandlersCollection(new TelegratorOptions());
        collection.AddDescriptor(new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler)));

        var provider = new HandlersProvider(collection, new TelegratorOptions());

        provider.IsEmpty().Should().BeFalse();
    }

    [Fact]
    public void TryGetDescriptorList_ShouldReturnTrue_WhenUpdateTypeExists()
    {
        var collection = new HandlersCollection(new TelegratorOptions());
        collection.AddDescriptor(new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler)));

        var provider = new HandlersProvider(collection, new TelegratorOptions());

        bool found = provider.TryGetDescriptorList(UpdateType.Message, out var list);
        found.Should().BeTrue();
        list.Should().NotBeNull();
        list!.Count.Should().Be(1);
    }

    [Fact]
    public void TryGetDescriptorList_ShouldReturnFalse_WhenUpdateTypeMissing()
    {
        var collection = new HandlersCollection(new TelegratorOptions());
        var provider = new HandlersProvider(collection, new TelegratorOptions());

        bool found = provider.TryGetDescriptorList(UpdateType.CallbackQuery, out var list);
        found.Should().BeFalse();
        list.Should().BeNull();
    }

    [Fact]
    public void GetHandlerInstance_ShouldCreateInstance_ForGeneralDescriptor()
    {
        var collection = new HandlersCollection(new TelegratorOptions());
        var descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        collection.AddDescriptor(descriptor);

        var provider = new HandlersProvider(collection, new TelegratorOptions());
        var instance = provider.GetHandlerInstance(descriptor);

        instance.Should().NotBeNull();
        instance.Should().BeOfType<TestUpdateHandler>();
    }

    [Fact]
    public void GetHandlerInstance_ShouldReturnSingleton_ForSingletonDescriptor()
    {
        var collection = new HandlersCollection(new TelegratorOptions());
        var handler = new TestUpdateHandler();
        var descriptor = new ClassHandlerDescriptor(DescriptorType.Singleton, typeof(TestUpdateHandler));
        descriptor.SetInstance(handler);
        collection.AddDescriptor(descriptor);

        var provider = new HandlersProvider(collection, new TelegratorOptions());
        var instance1 = provider.GetHandlerInstance(descriptor);
        var instance2 = provider.GetHandlerInstance(descriptor);

        instance1.Should().BeSameAs(handler);
        instance2.Should().BeSameAs(handler);
    }

    [Fact]
    public void GetHandlerInstance_ShouldUseFactory_WhenAddedViaAddHandler()
    {
        var collection = new HandlersCollection(new TelegratorOptions());
        collection.AddHandler<TestUpdateHandler>();

        var provider = new HandlersProvider(collection, new TelegratorOptions());
        var descriptor = provider.TryGetDescriptorList(UpdateType.Message, out var list) ? list![0] : null;

        descriptor.Should().NotBeNull();
        var instance = provider.GetHandlerInstance(descriptor!);

        instance.Should().NotBeNull();
        instance.Should().BeOfType<TestUpdateHandler>();
    }
}
