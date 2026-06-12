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

using FluentAssertions;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Handlers;
using Telegrator.Providers;
using Xunit;

namespace Telegrator.Tests.Providers;

public class HandlersProviderTests
{
    [Fact]
    public void IsEmpty_ShouldReturnTrue_ForEmptyCollection()
    {
        HandlersCollection collection = new HandlersCollection(new TelegratorOptions());
        HandlersProvider provider = new HandlersProvider(collection, new TelegratorOptions());

        provider.IsEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_ShouldReturnFalse_WhenHandlersExist()
    {
        HandlersCollection collection = new HandlersCollection(new TelegratorOptions());
        collection.AddDescriptor(new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler)));

        HandlersProvider provider = new HandlersProvider(collection, new TelegratorOptions());

        provider.IsEmpty().Should().BeFalse();
    }

    [Fact]
    public void TryGetDescriptorList_ShouldReturnTrue_WhenUpdateTypeExists()
    {
        HandlersCollection collection = new HandlersCollection(new TelegratorOptions());
        collection.AddDescriptor(new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler)));

        HandlersProvider provider = new HandlersProvider(collection, new TelegratorOptions());

        bool found = provider.TryGetDescriptorList(UpdateType.Message, out HandlerDescriptorList? list);
        found.Should().BeTrue();
        list.Should().NotBeNull();
        list!.Count.Should().Be(1);
    }

    [Fact]
    public void TryGetDescriptorList_ShouldReturnFalse_WhenUpdateTypeMissing()
    {
        HandlersCollection collection = new HandlersCollection(new TelegratorOptions());
        HandlersProvider provider = new HandlersProvider(collection, new TelegratorOptions());

        bool found = provider.TryGetDescriptorList(UpdateType.CallbackQuery, out HandlerDescriptorList? list);
        found.Should().BeFalse();
        list.Should().BeNull();
    }

    [Fact]
    public void GetHandlerInstance_ShouldCreateInstance_ForGeneralDescriptor()
    {
        HandlersCollection collection = new HandlersCollection(new TelegratorOptions());
        ClassHandlerDescriptor descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        collection.AddDescriptor(descriptor);

        HandlersProvider provider = new HandlersProvider(collection, new TelegratorOptions());
        UpdateHandlerBase instance = provider.GetHandlerInstance(descriptor);

        instance.Should().NotBeNull();
        instance.Should().BeOfType<TestUpdateHandler>();
    }

    [Fact]
    public void GetHandlerInstance_ShouldReturnSingleton_ForSingletonDescriptor()
    {
        HandlersCollection collection = new HandlersCollection(new TelegratorOptions());
        TestUpdateHandler handler = new TestUpdateHandler();
        ClassHandlerDescriptor descriptor = new ClassHandlerDescriptor(DescriptorType.Singleton, typeof(TestUpdateHandler));
        descriptor.SetInstance(handler);
        collection.AddDescriptor(descriptor);

        HandlersProvider provider = new HandlersProvider(collection, new TelegratorOptions());
        UpdateHandlerBase instance1 = provider.GetHandlerInstance(descriptor);
        UpdateHandlerBase instance2 = provider.GetHandlerInstance(descriptor);

        instance1.Should().BeSameAs(handler);
        instance2.Should().BeSameAs(handler);
    }

    [Fact]
    public void GetHandlerInstance_ShouldUseFactory_WhenAddedViaAddHandler()
    {
        HandlersCollection collection = new HandlersCollection(new TelegratorOptions());
        collection.AddHandler<TestUpdateHandler>();

        HandlersProvider provider = new HandlersProvider(collection, new TelegratorOptions());
        HandlerDescriptor? descriptor = provider.TryGetDescriptorList(UpdateType.Message, out HandlerDescriptorList? list) ? list![0] : null;

        descriptor.Should().NotBeNull();
        UpdateHandlerBase instance = provider.GetHandlerInstance(descriptor!);

        instance.Should().NotBeNull();
        instance.Should().BeOfType<TestUpdateHandler>();
    }
}
