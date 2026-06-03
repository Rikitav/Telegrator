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
using Telegrator.Core.Descriptors;
using Telegrator.Core.Filters;
using Xunit;

namespace Telegrator.Tests.Collections;

/// <summary>
/// РўРµСЃС‚С‹ РґР»СЏ РєРѕР»Р»РµРєС†РёР№.
///
/// РџРђР РђР”РР“РњР« РўР•РЎРўРР РћР’РђРќРРЇ:
/// 1. Collection Testing - С‚РµСЃС‚РёСЂРѕРІР°РЅРёРµ РєРѕР»Р»РµРєС†РёР№ Рё РёС… РѕРїРµСЂР°С†РёР№
/// 2. List Testing - С‚РµСЃС‚РёСЂРѕРІР°РЅРёРµ СЃРїРёСЃРєРѕРІ
/// 3. Indexing Testing - С‚РµСЃС‚РёСЂРѕРІР°РЅРёРµ РёРЅРґРµРєСЃР°С†РёРё
/// 4. Enumeration Testing - С‚РµСЃС‚РёСЂРѕРІР°РЅРёРµ РїРµСЂРµС‡РёСЃР»РµРЅРёСЏ
/// 5. Capacity Testing - С‚РµСЃС‚РёСЂРѕРІР°РЅРёРµ РµРјРєРѕСЃС‚Рё РєРѕР»Р»РµРєС†РёР№
/// </summary>
public class CollectionTests
{
    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - СЃРѕР·РґР°РЅРёРµ СЃРїРёСЃРєР°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј СЃРѕР·РґР°РЅРёРµ РєРѕР»Р»РµРєС†РёР№
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_ShouldBeCreated()
    {
        // Arrange & Act
        var list = new HandlerDescriptorList();

        // Assert
        list.Should().NotBeNull();
        list.Should().BeEmpty();
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - РґРѕР±Р°РІР»РµРЅРёРµ РґРµСЃРєСЂРёРїС‚РѕСЂР°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РґРѕР±Р°РІР»РµРЅРёРµ СЌР»РµРјРµРЅС‚РѕРІ РІ РєРѕР»Р»РµРєС†РёСЋ
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_Add_ShouldAddDescriptor()
    {
        // Arrange
        var list = new HandlerDescriptorList();
        var descriptor = CreateTestDescriptor();

        // Act
        list.Add(descriptor);

        // Assert
        list.Should().HaveCount(1);
        list.Should().Contain(descriptor);
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - РґРѕР±Р°РІР»РµРЅРёРµ РЅРµСЃРєРѕР»СЊРєРёС… РґРµСЃРєСЂРёРїС‚РѕСЂРѕРІ.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РјРЅРѕР¶РµСЃС‚РІРµРЅРЅС‹Рµ РѕРїРµСЂР°С†РёРё
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_AddMultiple_ShouldAddAllDescriptors()
    {
        // Arrange
        var list = new HandlerDescriptorList();
        var descriptor1 = CreateTestDescriptor();
        var descriptor2 = CreateTestDescriptor();
        var descriptor3 = CreateTestDescriptor();

        // Act
        list.Add(descriptor1);
        list.Add(descriptor2);
        list.Add(descriptor3);

        // Assert
        list.Should().HaveCount(3);
        list.Should().Contain(descriptor1);
        list.Should().Contain(descriptor2);
        list.Should().Contain(descriptor3);
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - РїРѕР»СѓС‡РµРЅРёРµ РїРѕ РёРЅРґРµРєСЃСѓ.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РёРЅРґРµРєСЃР°С†РёСЋ РєРѕР»Р»РµРєС†РёР№
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_Indexer_ShouldReturnDescriptorAtIndex()
    {
        // Arrange
        var descriptor = CreateTestDescriptor();
        var list = new HandlerDescriptorList
        {
            descriptor
        };

        // Act
        var result = list[0];

        // Assert
        result.Should().Be(descriptor);
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - РїРѕР»СѓС‡РµРЅРёРµ РїРѕ РЅРµРІРµСЂРЅРѕРјСѓ РёРЅРґРµРєСЃСѓ.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РёСЃРєР»СЋС‡РµРЅРёСЏ РїСЂРё РЅРµРєРѕСЂСЂРµРєС‚РЅРѕРј РґРѕСЃС‚СѓРїРµ
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(100)]
    public void HandlerDescriptorList_IndexerWithInvalidIndex_ShouldThrowArgumentOutOfRangeException(int invalidIndex)
    {
        // Arrange
        var list = new HandlerDescriptorList
        {
            CreateTestDescriptor()
        };

        // Act & Assert
        list.Invoking(l => _ = l[invalidIndex])
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - РїРµСЂРµС‡РёСЃР»РµРЅРёРµ СЌР»РµРјРµРЅС‚РѕРІ.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РїРµСЂРµС‡РёСЃР»РµРЅРёРµ РєРѕР»Р»РµРєС†РёР№
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_ShouldBeEnumerable()
    {
        // Arrange
        var descriptor1 = CreateTestDescriptor();
        var descriptor2 = CreateTestDescriptor();
        var list = new HandlerDescriptorList
        {
            descriptor1,
            descriptor2
        };

        // Act
        var enumeratedItems = list.ToList();

        // Assert
        enumeratedItems.Should().HaveCount(2);
        enumeratedItems.Should().Contain(descriptor1);
        enumeratedItems.Should().Contain(descriptor2);
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - РѕС‡РёСЃС‚РєР° СЃРїРёСЃРєР°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РѕС‡РёСЃС‚РєСѓ РєРѕР»Р»РµРєС†РёР№
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_Clear_ShouldRemoveAllDescriptors()
    {
        // Arrange
        var list = new HandlerDescriptorList
        {
            CreateTestDescriptor(),
            CreateTestDescriptor()
        };

        // Act
        list.Clear();

        // Assert
        list.Should().BeEmpty();
        list.Should().HaveCount(0);
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - РїСЂРѕРІРµСЂРєР° СЃРѕРґРµСЂР¶Р°РЅРёСЏ СЌР»РµРјРµРЅС‚Р°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РїРѕРёСЃРє РІ РєРѕР»Р»РµРєС†РёСЏС…
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_Contains_ShouldReturnCorrectResult()
    {
        // Arrange
        var list = new HandlerDescriptorList();
        var descriptor = CreateTestDescriptor();
        var nonExistentDescriptor = CreateTestDescriptor();

        // Act
        list.Add(descriptor);
        var containsExisting = list.Contains(descriptor);
        var containsNonExistent = list.Contains(nonExistentDescriptor);

        // Assert
        containsExisting.Should().BeTrue();
        containsNonExistent.Should().BeFalse();
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - СѓРґР°Р»РµРЅРёРµ СЌР»РµРјРµРЅС‚Р°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј СѓРґР°Р»РµРЅРёРµ СЌР»РµРјРµРЅС‚РѕРІ РёР· РєРѕР»Р»РµРєС†РёР№
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_Remove_ShouldRemoveDescriptor()
    {
        // Arrange
        var list = new HandlerDescriptorList();
        var descriptor = CreateTestDescriptor();
        list.Add(descriptor);

        // Act
        var removed = list.Remove(descriptor);

        // Assert
        removed.Should().BeTrue();
        list.Should().BeEmpty();
        list.Should().NotContain(descriptor);
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ HandlerDescriptorList - СѓРґР°Р»РµРЅРёРµ РЅРµСЃСѓС‰РµСЃС‚РІСѓСЋС‰РµРіРѕ СЌР»РµРјРµРЅС‚Р°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј СѓРґР°Р»РµРЅРёРµ РЅРµСЃСѓС‰РµСЃС‚РІСѓСЋС‰РёС… СЌР»РµРјРµРЅС‚РѕРІ
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_RemoveNonExistent_ShouldReturnFalse()
    {
        // Arrange
        var list = new HandlerDescriptorList();
        var nonExistentDescriptor = CreateTestDescriptor();

        // Act
        var removed = list.Remove(nonExistentDescriptor);

        // Assert
        removed.Should().BeFalse();
        list.Should().BeEmpty();
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ CompletedFiltersList - СЃРѕР·РґР°РЅРёРµ СЃРїРёСЃРєР°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј СЃРѕР·РґР°РЅРёРµ СЃРїРµС†РёР°Р»РёР·РёСЂРѕРІР°РЅРЅС‹С… РєРѕР»Р»РµРєС†РёР№
    /// </summary>
    [Fact]
    public void CompletedFiltersList_ShouldBeCreated()
    {
        // Arrange & Act
        var list = new CompletedFiltersList();

        // Assert
        list.Should().NotBeNull();
        list.Should().BeEmpty();
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ РїСЂРѕРІРµСЂРєРё РїСЂРѕРёР·РІРѕРґРёС‚РµР»СЊРЅРѕСЃС‚Рё РєРѕР»Р»РµРєС†РёР№.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј РїСЂРѕРёР·РІРѕРґРёС‚РµР»СЊРЅРѕСЃС‚СЊ РїСЂРё Р±РѕР»СЊС€РѕРј РєРѕР»РёС‡РµСЃС‚РІРµ СЌР»РµРјРµРЅС‚РѕРІ
    /// </summary>
    [Fact]
    public void HandlerDescriptorList_ShouldHandleLargeNumberOfItems()
    {
        // Arrange
        var list = new HandlerDescriptorList();
        var itemsCount = 1000;

        // Act
        for (int i = 0; i < itemsCount; i++)
        {
            list.Add(CreateTestDescriptor());
        }

        // Assert
        list.Should().HaveCount(itemsCount);
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ РїСЂРѕРІРµСЂРєРё РїРѕС‚РѕРєРѕР±РµР·РѕРїР°СЃРЅРѕСЃС‚Рё (Р±Р°Р·РѕРІС‹Р№ С‚РµСЃС‚).
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј Р±Р°Р·РѕРІСѓСЋ РїРѕС‚РѕРєРѕР±РµР·РѕРїР°СЃРЅРѕСЃС‚СЊ
    /// </summary>
    [Fact]
    public async Task HandlerDescriptorList_ShouldHandleConcurrentAccess()
    {
        // Arrange
        var list = new HandlerDescriptorList();
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 10; j++)
                {
                    list.Add(CreateTestDescriptor());
                }
            }));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        list.Should().HaveCount(100);
    }

    /// <summary>
    /// Р’СЃРїРѕРјРѕРіР°С‚РµР»СЊРЅС‹Р№ РјРµС‚РѕРґ РґР»СЏ СЃРѕР·РґР°РЅРёСЏ С‚РµСЃС‚РѕРІРѕРіРѕ РґРµСЃРєСЂРёРїС‚РѕСЂР°.
    /// </summary>
    private static ClassHandlerDescriptor CreateTestDescriptor()
    {
        return new ClassHandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
    }
}
