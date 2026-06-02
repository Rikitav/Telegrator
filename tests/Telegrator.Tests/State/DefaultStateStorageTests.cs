using FluentAssertions;
using Telegrator.States;
using Xunit;

namespace Telegrator.Tests.State;

public class DefaultStateStorageTests
{
    private readonly DefaultStateStorage _storage = new();

    [Fact]
    public async Task SetAsync_ShouldStoreValue()
    {
        await _storage.SetAsync("key1", 42);
        var value = await _storage.GetAsync<int>("key1");

        value.Should().Be(42);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDefault_WhenKeyMissing()
    {
        var value = await _storage.GetAsync<int>("missing");

        value.Should().Be(0);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenKeyMissingAndTypeIsReference()
    {
        var value = await _storage.GetAsync<string>("missing");

        value.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveKey()
    {
        await _storage.SetAsync("key2", "value");
        await _storage.DeleteAsync("key2");

        var value = await _storage.GetAsync<string>("key2");
        value.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenKeyMissing()
    {
        System.Func<Task> act = async () => await _storage.DeleteAsync("missing");

        await act.Should().ThrowAsync<System.Exception>().WithMessage("*Failed to remove key*");
    }

    [Fact]
    public async Task SetAsync_ShouldThrow_WhenKeyIsNull()
    {
        System.Func<Task> act = async () => await _storage.SetAsync<string>(null!, "value");

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("key");
    }

    [Fact]
    public async Task SetAsync_ShouldThrow_WhenStateIsNull()
    {
        System.Func<Task> act = async () => await _storage.SetAsync<string>("key", null!);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("state");
    }
}
