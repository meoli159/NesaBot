using Discord.WebSocket;
using Discord.Commands;
using Discord;
using Victoria;
using Microsoft.Extensions.DependencyInjection;
using NesaBot.Core.Managers;

namespace NesaBot.Core
{
    public class Bot
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;

        public Bot()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Debug
            });

            _commandService = new CommandService(new CommandServiceConfig()
            { 
                LogLevel= LogSeverity.Debug,
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                IgnoreExtraArgs = true
            });

            var collection = new ServiceCollection();

            collection.AddSingleton(_client);
            collection.AddSingleton(_commandService);
            collection.AddLavaNode(x => {
                x.SelfDeaf = false;
            });

            ServiceManager.SetProvider(collection);
        }

            public async Task MainAsync()
        {
            if (string.IsNullOrWhiteSpace(ConfigManager.Config.Token)) return;

            await CommandManager.LoadCommandsAsync();
            await EventManager.LoadCommands();
            await _client.LoginAsync(TokenType.Bot, ConfigManager.Config.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
