using Microsoft.Extensions.Hosting;
using Telegrator.Core;

namespace Telegrator.Hosting;

/// <summary>
/// Represents a hosted telegram bots and services builder that helps manage configuration, logging, lifetime, and more.
/// </summary>
public interface ITelegramBotHostBuilder : IHostApplicationBuilder, ICollectingProvider;
