using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VSEVerificationBot.Services;

// setup our fields we assign later
IConfigurationRoot _config;
DiscordSocketClient _client;
InteractionService _commands;
ulong _testGuildId;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables(prefix: "Railway_");

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var socketConfig = new DiscordSocketConfig()
{
    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
};

var socketClient = new DiscordSocketClient(socketConfig);

var services = builder.Services
    .AddSingleton(socketClient)
    .AddSingleton<CommandHandler>()
    .BuildServiceProvider();

_client = services.GetRequiredService<DiscordSocketClient>();
_commands = services.GetRequiredService<InteractionService>();
_config = services.GetRequiredService<IConfigurationRoot>();

_testGuildId = _config.GetSection("testGuildId").Get<ulong>();

_client.Log += LogAsync;
_commands.Log += LogAsync;
_client.Ready += ReadyAsync;

await services.GetRequiredService<CommandHandler>().InitializeAsync();

await _client.LoginAsync(TokenType.Bot, _config["Token"]);
await _client.StartAsync();

Task LogAsync(LogMessage log)
{
    Console.WriteLine(log.ToString());
    return Task.CompletedTask;
}

async Task ReadyAsync()
{
   
    if (IsDebug())
    {
        // this is where you put the id of the test discord guild
        System.Console.WriteLine($"In debug mode, adding commands to {_testGuildId}...");
        await _commands.RegisterCommandsToGuildAsync(_testGuildId);
    }
    else
    {
        // this method will add commands globally, but can take around an hour
        await _commands.RegisterCommandsGloballyAsync(true);
    } ;
}

// this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
static bool IsDebug()
{
#if DEBUG
    return true;
#else
                return false;
#endif
}

builder.Build();
await Task.Delay(Timeout.Infinite);