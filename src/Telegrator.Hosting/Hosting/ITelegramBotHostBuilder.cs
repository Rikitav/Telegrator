using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Telegrator.Core;

namespace Telegrator.Hosting;

public interface ITelegramBotHostBuilder : IHostApplicationBuilder, ICollectingProvider
{

}
