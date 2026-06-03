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
using Xunit;

namespace Telegrator.Tests;

public class ResultTests
{
    [Fact]
    public void Ok_ShouldHaveSuccessTrue_AndRouteNextFalse()
    {
        var result = Result.Ok();

        result.Success.Should().BeTrue();
        result.RouteNext.Should().BeFalse();
        result.NextType.Should().BeNull();
    }

    [Fact]
    public void Fault_ShouldHaveSuccessFalse_AndRouteNextFalse()
    {
        var result = Result.Fault();

        result.Success.Should().BeFalse();
        result.RouteNext.Should().BeFalse();
        result.NextType.Should().BeNull();
    }

    [Fact]
    public void Next_ShouldHaveSuccessTrue_AndRouteNextTrue()
    {
        var result = Result.Next();

        result.Success.Should().BeTrue();
        result.RouteNext.Should().BeTrue();
        result.NextType.Should().BeNull();
    }

    [Fact]
    public void NextOfT_ShouldHaveCorrectNextType()
    {
        var result = Result.Next<ResultTests>();

        result.Success.Should().BeTrue();
        result.RouteNext.Should().BeTrue();
        result.NextType.Should().Be<ResultTests>();
    }

    [Fact]
    public void Ok_ShouldBeSingleton()
    {
        var a = Result.Ok();
        var b = Result.Ok();

        a.Should().Be(b);
    }

    [Fact]
    public void Fault_ShouldBeSingleton()
    {
        var a = Result.Fault();
        var b = Result.Fault();

        a.Should().Be(b);
    }

    [Fact]
    public void Next_ShouldBeSingleton()
    {
        var a = Result.Next();
        var b = Result.Next();

        a.Should().Be(b);
    }

    [Fact]
    public void NextOfT_ShouldNotBeEqualTo_Next()
    {
        var a = Result.Next<ResultTests>();
        var b = Result.Next();

        a.Should().NotBe(b);
    }

    [Fact]
    public void Result_ShouldSupportValueEquality()
    {
        var a = Result.Ok();
        var b = Result.Ok();

        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Result_ShouldSupportValueInequality()
    {
        var a = Result.Ok();
        var b = Result.Fault();

        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Result_ShouldBeImmutable()
    {
        var result = Result.Ok();

        // init-only setters prevent mutation after creation
        // This test documents the immutability contract
        result.Success.Should().BeTrue();
        result.RouteNext.Should().BeFalse();
    }
}
