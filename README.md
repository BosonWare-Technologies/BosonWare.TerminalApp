# BosonWare.TerminalApp

BosonWare.TerminalApp is a modern, extensible .NET terminal application framework designed to simplify the creation of interactive command-line tools. It provides a robust command registry, built-in commands, command history, and flexible parsing utilities, making it ideal for building rich terminal experiences.

## Features

- **Command Registry**: Easily register and manage commands with attributes and aliases.
- **Built-in Commands**: Includes common commands like `help`, `clear`, `exit`, `version`, and `time`.
- **Command History**: Persistent command history with disk storage.
- **Command Parsing**: Advanced command-line parsing with support for quoted arguments.
- **Extensibility**: Add custom commands by implementing the `ICommand` interface or deriving from `Command<TOptions>`.
- **Colorful Output**: Integrates with BosonWare.TUI for styled console output.

## Getting Started

### Prerequisites

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later

### Installation

Add the NuGet package to your project:

```sh
dotnet add package BosonWare.TerminalApp
```

### Example Usage

Create a simple terminal app:

```csharp
using BosonWare.TerminalApp;
using BosonWare.TerminalApp.BuiltIn;

CommandRegistry.LoadCommands<Program>(typeof(IBuiltInAssemblyMarker));

var app = new ConsoleApplication() {
    Prompt = "[Crimson]Boson Terminal[/] > ",
    History = CommandHistory.CreateAsync("command_history.json").GetAwaiter().GetResult()
};

await app.RunAsync();
```

### Registering Commands

Commands are registered using the `[Command]` attribute:

```csharp
[Command("greet", Aliases = ["hello"], Description = "Greets the user.")]
public sealed class GreetCommand : ICommand
{
    public Task Execute(string arguments)
    {
        Console.WriteLine($"Hello, {arguments}!");
        return Task.CompletedTask;
    }
}
```

Or use options parsing:

```csharp
public class EchoOptions
{
    [Option('u', "upper", HelpText = "Uppercase output.")]
    public bool Upper { get; set; }
}

[Command("echo", Description = "Echoes input.")]
public sealed class EchoCommand : Command<EchoOptions>
{
    public override Task Execute(EchoOptions options)
    {
        // Implementation here
        return Task.CompletedTask;
    }
}
```

### Built-in Commands

- `help` / `info` / `-h` / `?` — Displays help
- `clear` / `cls` — Clears the terminal
- `exit` / `shutdown` — Exits the application
- `version` / `ver` — Shows version info
- `time` — Shows current time (`--utc` for UTC)

## Command History

Command history is persisted to disk and loaded on startup. The default limit is 1000 entries.

## License

MIT License. See [LICENSE](./LICENSE) for details.

## Contributing

Contributions are welcome! Please submit issues or pull requests via [GitHub](https://github.com/BosonWare-Technologies/BosonWare.TerminalApp).

## Links

- [BosonWare.TerminalApp on GitHub](https://github.com/BosonWare-Technologies/BosonWare.TerminalApp)
- [BosonWare.Runtime](https://www.nuget.org/packages/BosonWare.Runtime)
- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser)
