using BosonWare.TerminalApp.BuiltIn;
using BosonWare.TUI;

namespace BosonWare.TerminalApp;

public sealed class ConsoleApplication
{
    private readonly bool _isRunning = true;

    public required CommandHistory History { get; init; }

    public string Prompt { get; set; } = "";

    public static void HandleCancelKeyPress(object? sender, ConsoleCancelEventArgs e) =>
        // Ignore Ctr+C commands.
        e.Cancel = true;

    public async Task RunAsync()
    {
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

    private static async Task<bool> ExecCommand(string userCommand)
    {
        var (CommandName, Args) = CommandLineParser.Parse(userCommand);

        var command = CommandRegistry.GetCommand(CommandName);

        if (command is null) {
            SmartConsole.LogError("Unknown command");

            return false;
        }

        bool status = false;
        // Safely execute the command.
        try {
            await command.Execute(Args);

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

    public static async Task<ConsoleApplication> CreateAsync()
    {
        var path = Application.GetPath("history.json");

        var history = await CommandHistory.CreateAsync(path);

        return new ConsoleApplication() {
            History = history
        };
    }
}