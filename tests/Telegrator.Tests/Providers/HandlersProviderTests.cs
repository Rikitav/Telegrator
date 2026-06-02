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
