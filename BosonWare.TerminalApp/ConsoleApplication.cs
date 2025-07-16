using BosonWare.TerminalApp.BuiltIn;
using BosonWare.TUI;

namespace BosonWare.TerminalApp;

public sealed class ConsoleApplication
{
    public static ConsoleApplication Current { get; private set; } = null!;
    
    private readonly Dictionary<string, MinimalCommand>  _commands = new();

    private volatile bool _isRunning;
    
    public required CommandHistory History { get; init; }

    public string Prompt { get; set; } = "";
    
    public IEnumerable<MinimalCommand> MinimalCommands => _commands.Values;

    public ConsoleApplication(string prompt)
    {
        Prompt = prompt;
        
        Current = this;
    }
    
    public MinimalCommand AddCommand(string name, Action<string> action, string description = "")
    {
        var command = new MinimalCommand(name, description, action);
        
        _commands.Add(name, command);
        
        return command;
    }

    private static void HandleCancelKeyPress(object? sender, ConsoleCancelEventArgs e) =>
        // Ignore Ctr+C commands.
        e.Cancel = true;

    public async Task RunAsync()
    {
        _isRunning = true;
        
        Console.CancelKeyPress += HandleCancelKeyPress;

        while (_isRunning) {
            var userCommand = TUIConsole.ReadLineWithHistory(Prompt, History.History);

            await History.History.WriteToDiskAsync();

            Console.WriteLine();

            if (string.IsNullOrEmpty(userCommand)) {
                continue;
            }

            var commands = userCommand.Split("&&");

            foreach (var command in commands) {
                var status = await ExecCommand(command);

                if (!status) {
                    break;
                }
            }
        }
    }

    private async Task<bool> ExecCommand(string userCommand)
    {
        var (commandName, args) = CommandLineParser.Parse(userCommand);

        var command = _commands.TryGetValue(commandName, out var minimalCommand) 
            ? minimalCommand 
            : CommandRegistry.GetCommand(commandName);

        if (command is null) {
            SmartConsole.LogError("Unknown command");

            return false;
        }

        var status = false;
        // Safely execute the command.
        try {
            await command.Execute(args);

            status = true;
        }
        catch (Exception ex) {
            SmartConsole.LogError($"{ex.GetType().Name}: {ex.Message}");
        }

        // Safely dispose the command if it is disposable.
        if (command is IDisposable disposable) {
            try {
                disposable.Dispose();
            }
            catch (Exception ex) {
                SmartConsole.LogError($"[DisposeFailed] {ex.GetType().Name}: {ex.Message}");

                status = false;
            }
        }

        if (command is not ClearCommand) {
            Console.WriteLine();
        }

        return status;
    }

    public static async Task<ConsoleApplication> CreateAsync(string prompt = "Console")
    {
        var path = Application.GetPath("history.json");

        var history = await CommandHistory.CreateAsync(path);

        return new ConsoleApplication(prompt) {
            History = history
        };
    }
}
