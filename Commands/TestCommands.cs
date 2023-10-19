using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using VSEVerificationBot.Services;

namespace VSEVerificationBot.Commands;

public class TestCommands : InteractionModuleBase<SocketInteractionContext>
{
    private InteractionService _commands;
    private IConfigurationRoot _config;
    private CommandHandler _handler;
    private DiscordSocketClient _client;

    public TestCommands(IConfigurationRoot config, DiscordSocketClient client, CommandHandler handler, InteractionService commands)
    {
        _config = config;
        _client = client;
        _handler = handler;
        _commands = commands;
    }

    [SlashCommand("ping", "Test command to see if bot is alive")]
    public async Task Ping()
    {
        await RespondAsync("Ano, žiju :-)");
    }
}