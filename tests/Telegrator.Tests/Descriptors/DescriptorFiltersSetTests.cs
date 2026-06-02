using FluentAssertions;
using Moq;
using Telegram.Bot.Types;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;
using Telegrator.Filters;
using Telegrator.Handlers;
using Telegrator.Handlers.Diagnostics;
using Xunit;

namespace Telegrator.Tests.Descriptors;

public class DescriptorFiltersSetTests
{
    [Fact]
    public void Validate_ShouldReturnOk_WhenNoFilters()
    {
        var filtersSet = new DescriptorFiltersSet(null, null, null);
        var context = CreateContext();
        var descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        var report = new FiltersFallbackReport(descriptor, context);

        var result = filtersSet.Validate(context, false, ref report);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnOk_WhenAllFiltersPass()
    {
        var filter = Filter<Update>.If(_ => true);
        var filtersSet = new DescriptorFiltersSet(null, null, [filter]);
        var context = CreateContext();
        var descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        var report = new FiltersFallbackReport(descriptor, context);

        var result = filtersSet.Validate(context, false, ref report);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnFault_WhenFilterFails()
    {
        var filter = Filter<Update>.If(_ => false);
        var filtersSet = new DescriptorFiltersSet(null, null, [filter]);
        var context = CreateContext();
        var descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        var report = new FiltersFallbackReport(descriptor, context);

        var result = filtersSet.Validate(context, false, ref report);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnNext_WhenRouteNextModifierIsUsed()
    {
        var filter = new Mock<IFilter<Update>>();
        filter.Setup(f => f.CanPass(It.IsAny<FilterExecutionContext<Update>>())).Returns(true);

        var filtersSet = new DescriptorFiltersSet(
            new MessageHandlerAttribute { FormReport = true },
            null,
            [filter.Object]);

        var context = CreateContext();
        var descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        var report = new FiltersFallbackReport(descriptor, context);
        var result = filtersSet.Validate(context, true, ref report);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ExecutePre_ShouldReturnOk_WhenNoAspects()
    {
        var aspectsSet = new DescriptorAspectsSet(null, null);
        var handler = new TestUpdateHandler();
        var container = new Mock<IHandlerContainer>().Object;

        var result = await aspectsSet.ExecutePre(handler, container, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ExecutePost_ShouldReturnOk_WhenNoAspects()
    {
        var aspectsSet = new DescriptorAspectsSet(null, null);
        var handler = new TestUpdateHandler();
        var container = new Mock<IHandlerContainer>().Object;

        var result = await aspectsSet.ExecutePost(handler, container, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    private static FilterExecutionContext<Update> CreateContext()
    {
        return new FilterExecutionContext<Update>(
            new Mock<IUpdateRouter>().Object,
            new TelegramBotInfo(new User { Id = 1, IsBot = true, FirstName = "Test" }),
            new Update { Message = new Message { Text = "test" } },
            new Update { Message = new Message { Text = "test" } },
            new Dictionary<string, object>(),
            new CompletedFiltersList());
    }
}
