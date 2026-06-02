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
        var provider = new AwaitingProvider(new TelegratorOptions());

        bool found = provider.TryGetDescriptorList(UpdateType.Message, out var list);

        found.Should().BeFalse();
        list.Should().BeNull();
    }

    [Fact]
    public void TryGetDescriptorList_ShouldReturnTrue_WhenHandlerRegistered()
    {
        var provider = new AwaitingProvider(new TelegratorOptions());
        var handler = CreateTestHandlerDescriptor();

        using (provider.UseHandler(handler))
        {
            // AwaitingProvider is designed to register handlers dynamically and does not ties to a specific update type, so we can check for `UpdateType.Unknown` here.
            bool found = provider.TryGetDescriptorList(UpdateType.Unknown, out var list);
            found.Should().BeTrue();
            list.Should().NotBeNull();
            list!.Count.Should().Be(1);
        }
    }

    [Fact]
    public void UseHandler_ShouldAutoRemoveOnDispose()
    {
        var provider = new AwaitingProvider(new TelegratorOptions());
        var handler = CreateTestHandlerDescriptor();

        using (provider.UseHandler(handler))
        { }

        bool found = provider.TryGetDescriptorList(UpdateType.Message, out var list);
        found.Should().BeFalse();
        list.Should().BeNull();
    }

    [Fact]
    public void UseHandler_ShouldThrow_WhenDescriptorHasNoSingletonInstance()
    {
        var provider = new AwaitingProvider(new TelegratorOptions());
        var descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));

        System.Action act = () => provider.UseHandler(descriptor);

        act.Should().Throw<System.Exception>().WithMessage("*singleton instance*");
    }

    private static HandlerDescriptor CreateTestHandlerDescriptor()
    {
        var descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        descriptor.SetInstance(new TestUpdateHandler());
        return descriptor;
    }
}
