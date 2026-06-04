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
using System.Collections.ObjectModel;
using System.Reflection;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers.Building;
using Telegrator.Handlers;
using Xunit;

namespace Telegrator.Tests;

public class SimpleTypesExtensionsTests
{
    #region Collection Extensions

    [Fact]
    public void ToReadOnlyDictionary_ShouldCreateDictionary()
    {
        var items = new[] { "a", "bb", "ccc" };
        ReadOnlyDictionary<int, string> dict = items.ToReadOnlyDictionary(x => x.Length);

        dict.Should().ContainKey(1).WhoseValue.Should().Be("a");
        dict.Should().ContainKey(2).WhoseValue.Should().Be("bb");
        dict.Should().ContainKey(3).WhoseValue.Should().Be("ccc");
    }

    [Fact]
    public void Squeeze_ShouldRemoveNulls()
    {
        var items = new List<string?> { "a", null, "b", null, "c" };
        var squeezed = items.Squeeze().ToList();

        squeezed.Should().Equal("a", "b", "c");
    }

    [Fact]
    public void ForEach_ShouldExecuteAction()
    {
        var items = new List<int> { 1, 2, 3 };
        var sum = 0;
        items.ForEach(x => sum += x);

        sum.Should().Be(6);
    }

    [Fact]
    public void Set_ShouldUpdateExistingKey()
    {
        var dict = new Dictionary<string, int> { { "a", 1 } };
        dict.Set("a", 2);

        dict["a"].Should().Be(2);
    }

    [Fact]
    public void Set_ShouldAddNewKey()
    {
        var dict = new Dictionary<string, int>();
        dict.Set("a", 1);

        dict.Should().ContainKey("a").WhoseValue.Should().Be(1);
    }

    [Fact]
    public void Set_WithDefaultValue_ShouldAddDefaultWhenKeyMissing()
    {
        var dict = new Dictionary<string, int>();
        dict.Set("a", 10, 99);

        dict["a"].Should().Be(99);
    }

    [Fact]
    public void Random_ShouldReturnElementFromCollection()
    {
        var items = new List<int> { 1, 2, 3, 4, 5 };
        var result = items.Random();

        items.Should().Contain(result);
    }

    [Fact]
    public void UnionAdd_ShouldAddOnlyUniqueElements()
    {
        var list = new List<int> { 1, 2 };
        list.UnionAdd([2, 3, 4]);

        list.Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
        var items = new List<int> { 10, 20, 30 };
        items.IndexOf(x => x == 20).Should().Be(1);
        items.IndexOf(x => x == 99).Should().Be(-1);
    }

    [Fact]
    public void SingleOrNothing_ShouldReturnElement_WhenSingle()
    {
        var items = new List<int> { 42 };
        items.SingleOrNothing().Should().Be(42);
    }

    [Fact]
    public void SingleOrNothing_ShouldReturnDefault_WhenEmpty()
    {
        var items = new List<int>();
        items.SingleOrNothing().Should().Be(0);
    }

    [Fact]
    public void SingleOrNothing_ShouldReturnDefault_WhenMultiple()
    {
        var items = new List<int> { 1, 2 };
        items.SingleOrNothing().Should().Be(0);
    }

    [Fact]
    public void SingleOrNothing_WithPredicate_ShouldReturnElement_WhenSingleMatch()
    {
        var items = new List<int> { 1, 2, 3 };
        items.SingleOrNothing(x => x == 2).Should().Be(2);
    }

    #endregion

    #region Reflection Extensions

    [Fact]
    public void IsCustomDescriptorsProvider_ShouldReturnFalse_ForRegularType()
    {
        typeof(string).IsCustomDescriptorsProvider().Should().BeFalse();
    }

    [Fact]
    public void IsFilterType_ShouldReturnTrue_ForFilterImplementation()
    {
        typeof(UpdateValidateFilter).IsFilterType().Should().BeTrue();
    }

    [Fact]
    public void IsFilterType_ShouldReturnFalse_ForNonFilter()
    {
        typeof(string).IsFilterType().Should().BeFalse();
    }

    [Fact]
    public void IsHandlerAbstract_ShouldReturnTrue_ForAbstractHandler()
    {
        typeof(MessageHandler).IsHandlerAbstract().Should().BeTrue();
    }

    [Fact]
    public void IsHandlerImplementation_ShouldReturnTrue_ForConcreteHandler()
    {
        typeof(TestUpdateHandler).IsHandlerImplementation().Should().BeTrue();
    }

    [Fact]
    public void HasParameterlessCtor_ShouldReturnTrue_ForTypeWithDefaultCtor()
    {
        typeof(TestUpdateHandler).HasParameterlessCtor().Should().BeTrue();
    }

    [Fact]
    public void HasPublicProperties_ShouldReturnTrue_ForTypeWithProperties()
    {
        typeof(TestUpdateHandler).HasPublicProperties().Should().BeTrue();
    }

    [Fact]
    public void IsAssignableToGenericType_ShouldReturnTrue_ForGenericInterface()
    {
        typeof(UpdateValidateFilter).IsAssignableToGenericType(typeof(IFilter<>)).Should().BeTrue();
    }

    [Fact]
    public void IsAssignableToGenericType_ShouldReturnFalse_ForUnrelatedType()
    {
        typeof(string).IsAssignableToGenericType(typeof(IFilter<>)).Should().BeFalse();
    }

    #endregion

    #region String Extensions

    [Fact]
    public void SliceBy_ShouldSplitStringIntoChunks()
    {
        var chunks = "HelloWorld".SliceBy(3).ToList();
        chunks.Should().Equal("Hel", "loW", "orl", "d");
    }

    [Fact]
    public void SliceBy_ShouldHandleEmptyString()
    {
        "".SliceBy(3).Should().BeEmpty();
    }

    [Fact]
    public void FirstLetterToUpper_ShouldUppercaseFirstLetter()
    {
        "hello".FirstLetterToUpper().Should().Be("Hello");
    }

    [Fact]
    public void FirstLetterToLower_ShouldLowercaseFirstLetter()
    {
        "Hello".FirstLetterToLower().Should().Be("hello");
    }

    [Fact]
    public void ContainsWord_ShouldReturnTrue_WhenWordIsStandalone()
    {
        "hello world".ContainsWord("world").Should().BeTrue();
    }

    [Fact]
    public void ContainsWord_ShouldReturnFalse_WhenWordIsPartOfAnother()
    {
        "hello world".ContainsWord("wor").Should().BeFalse();
    }

    [Fact]
    public void ContainsWord_ShouldReturnFalse_WhenNotFound()
    {
        "hello world".ContainsWord("foo").Should().BeFalse();
    }

    #endregion

    #region Number Extensions

    [Fact]
    public void HasFlag_ShouldReturnTrue_WhenFlagIsSet()
    {
        0b1010.HasFlag(0b0010).Should().BeTrue();
    }

    [Fact]
    public void HasFlag_ShouldReturnFalse_WhenFlagIsNotSet()
    {
        0b1010.HasFlag(0b0100).Should().BeFalse();
    }

    [Fact]
    public void HasFlag_Generic_ShouldWorkWithEnum()
    {
        BindingFlags value = BindingFlags.Public | BindingFlags.Instance;
        value.HasFlag(BindingFlags.Public).Should().BeTrue();
        value.HasFlag(BindingFlags.Static).Should().BeFalse();
    }

    #endregion
}
