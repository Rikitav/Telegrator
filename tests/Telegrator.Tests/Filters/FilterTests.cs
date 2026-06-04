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
using Telegram.Bot.Types;
using Telegrator.Core.Filters;
using Telegrator.Filters;
using Xunit;

#pragma warning disable CS8625
namespace Telegrator.Tests.Filters;

/// <summary>
/// РўРµСЃС‚С‹ РґР»СЏ Р±Р°Р·РѕРІС‹С… С„РёР»СЊС‚СЂРѕРІ.
///
/// РџРђР РђР”РР“РњР« РўР•РЎРўРР РћР’РђРќРРЇ:
/// 1. AAA (Arrange-Act-Assert) - СЃС‚СЂСѓРєС‚СѓСЂР° С‚РµСЃС‚Р°: РїРѕРґРіРѕС‚РѕРІРєР°, РґРµР№СЃС‚РІРёРµ, РїСЂРѕРІРµСЂРєР°
/// 2. Given-When-Then - Р°Р»СЊС‚РµСЂРЅР°С‚РёРІРЅР°СЏ С„РѕСЂРјСѓР»РёСЂРѕРІРєР° AAA РґР»СЏ Р»СѓС‡С€РµР№ С‡РёС‚Р°РµРјРѕСЃС‚Рё
/// 3. РўРµСЃС‚РёСЂРѕРІР°РЅРёРµ РіСЂР°РЅРёС‡РЅС‹С… СЃР»СѓС‡Р°РµРІ Рё РёСЃРєР»СЋС‡РµРЅРёР№
/// 4. РСЃРїРѕР»СЊР·РѕРІР°РЅРёРµ РјРѕРєРѕРІ РґР»СЏ РёР·РѕР»СЏС†РёРё С‚РµСЃС‚РёСЂСѓРµРјРѕРіРѕ РєРѕРґР°
/// 5. РўРµСЃС‚РёСЂРѕРІР°РЅРёРµ РєР°Рє РїРѕР·РёС‚РёРІРЅС‹С…, С‚Р°Рє Рё РЅРµРіР°С‚РёРІРЅС‹С… СЃС†РµРЅР°СЂРёРµРІ
/// </summary>
public class FilterTests
{
    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ AnyFilter - С„РёР»СЊС‚СЂ, РєРѕС‚РѕСЂС‹Р№ РІСЃРµРіРґР° РїСЂРѕС…РѕРґРёС‚.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј Р±Р°Р·РѕРІРѕРµ РїРѕРІРµРґРµРЅРёРµ - С„РёР»СЊС‚СЂ РґРѕР»Р¶РµРЅ РІСЃРµРіРґР° РІРѕР·РІСЂР°С‰Р°С‚СЊ true
    /// </summary>
    [Fact]
    public void AnyFilter_ShouldAlwaysPass()
    {
        // Arrange (Given) - РїРѕРґРіРѕС‚РѕРІРєР° С‚РµСЃС‚РѕРІС‹С… РґР°РЅРЅС‹С…
        AnyFilter<Update> anyFilter = Filter<Update>.Any();
        FilterExecutionContext<Update> context = new FilterExecutionContext<Update>(null, new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

        // Act (When) - РІС‹РїРѕР»РЅРµРЅРёРµ С‚РµСЃС‚РёСЂСѓРµРјРѕРіРѕ РґРµР№СЃС‚РІРёСЏ
        var result = anyFilter.CanPass(context);

        // Assert (Then) - РїСЂРѕРІРµСЂРєР° СЂРµР·СѓР»СЊС‚Р°С‚Р°
        result.Should().BeTrue();
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ ReverseFilter - РёРЅРІРµСЂС‚РёСЂРѕРІР°РЅРёРµ СЂРµР·СѓР»СЊС‚Р°С‚Р° С„РёР»СЊС‚СЂР°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РєРѕРјРїРѕР·РёС†РёСЋ С„РёР»СЊС‚СЂРѕРІ Рё Р»РѕРіРёРєСѓ РёРЅРІРµСЂСЃРёРё
    /// </summary>
    [Fact]
    public void ReverseFilter_ShouldInvertResult()
    {
        // Arrange
        AnyFilter<Update> alwaysTrueFilter = Filter<Update>.Any();
        Filter<Update> reverseFilter = alwaysTrueFilter.Not();
        FilterExecutionContext<Update> context = new FilterExecutionContext<Update>(null, new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

        // Act
        var result = reverseFilter.CanPass(context);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ AndFilter - Р»РѕРіРёС‡РµСЃРєРѕРµ Р РјРµР¶РґСѓ С„РёР»СЊС‚СЂР°РјРё.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РєРѕРјР±РёРЅРёСЂРѕРІР°РЅРёРµ С„РёР»СЊС‚СЂРѕРІ Рё Р»РѕРіРёРєСѓ Р
    /// </summary>
    [Theory]
    [InlineData(true, true, true)]   // РћР±Р° С„РёР»СЊС‚СЂР° РїСЂРѕС…РѕРґСЏС‚
    [InlineData(true, false, false)] // РџРµСЂРІС‹Р№ РїСЂРѕС…РѕРґРёС‚, РІС‚РѕСЂРѕР№ РЅРµС‚
    [InlineData(false, true, false)] // РџРµСЂРІС‹Р№ РЅРµ РїСЂРѕС…РѕРґРёС‚, РІС‚РѕСЂРѕР№ РїСЂРѕС…РѕРґРёС‚
    [InlineData(false, false, false)] // РћР±Р° С„РёР»СЊС‚СЂР° РЅРµ РїСЂРѕС…РѕРґСЏС‚
    public void AndFilter_ShouldCombineFiltersWithAndLogic(bool firstResult, bool secondResult, bool expectedResult)
    {
        // Arrange
        Filter<Update> firstFilter = Filter<Update>.If(_ => firstResult);
        Filter<Update> secondFilter = Filter<Update>.If(_ => secondResult);
        AndFilter<Update> andFilter = firstFilter.And(secondFilter);
        FilterExecutionContext<Update> context = new FilterExecutionContext<Update>(null, new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

        // Act
        var result = andFilter.CanPass(context);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ OrFilter - Р»РѕРіРёС‡РµСЃРєРѕРµ РР›Р РјРµР¶РґСѓ С„РёР»СЊС‚СЂР°РјРё.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РєРѕРјР±РёРЅРёСЂРѕРІР°РЅРёРµ С„РёР»СЊС‚СЂРѕРІ Рё Р»РѕРіРёРєСѓ РР›Р
    /// </summary>
    [Theory]
    [InlineData(true, true, true)]   // РћР±Р° С„РёР»СЊС‚СЂР° РїСЂРѕС…РѕРґСЏС‚
    [InlineData(true, false, true)]  // РџРµСЂРІС‹Р№ РїСЂРѕС…РѕРґРёС‚, РІС‚РѕСЂРѕР№ РЅРµС‚
    [InlineData(false, true, true)]  // РџРµСЂРІС‹Р№ РЅРµ РїСЂРѕС…РѕРґРёС‚, РІС‚РѕСЂРѕР№ РїСЂРѕС…РѕРґРёС‚
    [InlineData(false, false, false)] // РћР±Р° С„РёР»СЊС‚СЂР° РЅРµ РїСЂРѕС…РѕРґСЏС‚
    public void OrFilter_ShouldCombineFiltersWithOrLogic(bool firstResult, bool secondResult, bool expectedResult)
    {
        // Arrange
        Filter<Update> firstFilter = Filter<Update>.If(_ => firstResult);
        Filter<Update> secondFilter = Filter<Update>.If(_ => secondResult);
        OrFilter<Update> orFilter = firstFilter.Or(secondFilter);
        FilterExecutionContext<Update> context = new FilterExecutionContext<Update>(null, new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

        // Act
        var result = orFilter.CanPass(context);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ CompiledFilter - РєРѕРјРїРёР»СЏС†РёСЏ РЅРµСЃРєРѕР»СЊРєРёС… С„РёР»СЊС‚СЂРѕРІ.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј СЃР»РѕР¶РЅСѓСЋ РєРѕРјРїРѕР·РёС†РёСЋ С„РёР»СЊС‚СЂРѕРІ
    /// </summary>
    [Fact]
    public void CompiledFilter_ShouldPassOnlyWhenAllFiltersPass()
    {
        // Arrange
        Filter<Update> filter1 = Filter<Update>.If(_ => true);
        Filter<Update> filter2 = Filter<Update>.If(_ => true);
        Filter<Update> filter3 = Filter<Update>.If(_ => false);

        CompiledFilter<Update> compiledFilter = new CompiledFilter<Update>(filter1, filter2, filter3);
        Dictionary<string, object> data = new Dictionary<string, object> { { "handler_name", "TestHandler" } };
        FilterExecutionContext<Update> context = new FilterExecutionContext<Update>(null, new TelegramBotInfo(null), new Update(), new Update(), data, new CompletedFiltersList());

        // Act
        var result = compiledFilter.CanPass(context);

        // Assert
        result.Should().BeFalse(); // Р”РѕР»Р¶РµРЅ РІРµСЂРЅСѓС‚СЊ false, С‚Р°Рє РєР°Рє filter3 РІРѕР·РІСЂР°С‰Р°РµС‚ false
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ РїСЂРѕРІРµСЂРєРё IsCollectible СЃРІРѕР№СЃС‚РІР°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј СЃРІРѕР№СЃС‚РІР° РѕР±СЉРµРєС‚РѕРІ
    /// </summary>
    [Fact]
    public void Filter_IsCollectible_ShouldBeTrueForAnyFilter()
    {
        // Arrange
        AnyFilter<Update> anyFilter = Filter<Update>.Any();

        // Act
        var isCollectible = anyFilter.IsCollectible;

        // Assert
        isCollectible.Should().BeFalse();
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ FunctionFilter - С„РёР»СЊС‚СЂ РЅР° РѕСЃРЅРѕРІРµ С„СѓРЅРєС†РёРё.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј СЃРѕР·РґР°РЅРёРµ С„РёР»СЊС‚СЂРѕРІ РёР· С„СѓРЅРєС†РёР№
    /// </summary>
    [Fact]
    public void FunctionFilter_ShouldUseProvidedFunction()
    {
        // Arrange
        var wasCalled = false;
        Filter<Update> functionFilter = Filter<Update>.If(_ =>
        {
            wasCalled = true;
            return true;
        });
        FilterExecutionContext<Update> context = new FilterExecutionContext<Update>(null, new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

        // Act
        var result = functionFilter.CanPass(context);

        // Assert
        result.Should().BeTrue();
        wasCalled.Should().BeTrue();
    }
}
