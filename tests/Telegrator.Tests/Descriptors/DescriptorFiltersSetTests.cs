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
        DescriptorFiltersSet filtersSet = new DescriptorFiltersSet(null, null, null);
        FilterExecutionContext<Update> context = CreateContext();
        ClassHandlerDescriptor descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        FiltersFallbackReport report = new FiltersFallbackReport(descriptor, context);

        Result result = filtersSet.Validate(context, false, ref report);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnOk_WhenAllFiltersPass()
    {
        Filter<Update> filter = Filter<Update>.If(_ => true);
        DescriptorFiltersSet filtersSet = new DescriptorFiltersSet(null, null, [filter]);
        FilterExecutionContext<Update> context = CreateContext();
        ClassHandlerDescriptor descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        FiltersFallbackReport report = new FiltersFallbackReport(descriptor, context);

        Result result = filtersSet.Validate(context, false, ref report);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnFault_WhenFilterFails()
    {
        Filter<Update> filter = Filter<Update>.If(_ => false);
        DescriptorFiltersSet filtersSet = new DescriptorFiltersSet(null, null, [filter]);
        FilterExecutionContext<Update> context = CreateContext();
        ClassHandlerDescriptor descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        FiltersFallbackReport report = new FiltersFallbackReport(descriptor, context);

        Result result = filtersSet.Validate(context, false, ref report);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnNext_WhenRouteNextModifierIsUsed()
    {
        Mock<IFilter<Update>> filter = new Mock<IFilter<Update>>();
        filter.Setup(f => f.CanPass(It.IsAny<FilterExecutionContext<Update>>())).Returns(true);

        DescriptorFiltersSet filtersSet = new DescriptorFiltersSet(
            new MessageHandlerAttribute { FormReport = true },
            null,
            [filter.Object]);

        FilterExecutionContext<Update> context = CreateContext();
        ClassHandlerDescriptor descriptor = new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        FiltersFallbackReport report = new FiltersFallbackReport(descriptor, context);
        Result result = filtersSet.Validate(context, true, ref report);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ExecutePre_ShouldReturnOk_WhenNoAspects()
    {
        DescriptorAspectsSet aspectsSet = new DescriptorAspectsSet(null, null);
        TestUpdateHandler handler = new TestUpdateHandler();
        IHandlerContainer container = new Mock<IHandlerContainer>().Object;

        Result result = await aspectsSet.ExecutePre(handler, container, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ExecutePost_ShouldReturnOk_WhenNoAspects()
    {
        DescriptorAspectsSet aspectsSet = new DescriptorAspectsSet(null, null);
        TestUpdateHandler handler = new TestUpdateHandler();
        IHandlerContainer container = new Mock<IHandlerContainer>().Object;

        Result result = await aspectsSet.ExecutePost(handler, container, CancellationToken.None);

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
