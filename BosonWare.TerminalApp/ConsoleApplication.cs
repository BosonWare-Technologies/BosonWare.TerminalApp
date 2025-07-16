using BosonWare.TerminalApp.BuiltIn;
using BosonWare.TUI;

namespace BosonWare.TerminalApp;

/// <summary>
/// Represents a console application that manages commands, command history, and user interaction via a terminal interface.
/// </summary>
public sealed class ConsoleApplication
{
    /// <summary>
    /// Gets the current instance of the <see cref="ConsoleApplication"/>.
    /// </summary>
    public static ConsoleApplication Current { get; private set; } = null!;

    /// <summary>
    /// Stores the registered minimal commands by their names.
    /// </summary>
    private readonly Dictionary<string, IMinimalCommand> _commands = [];

    /// <summary>
    /// Indicates whether the application is currently running.
    /// </summary>
    private volatile bool _isRunning;

    public TerminationMode TerminationMode { get; set; } = TerminationMode.TerminateOnCtrlC;

    /// <summary>
    /// Gets or sets the command history for the application.
    /// </summary>
    public required CommandHistory History { get; init; }

    /// <summary>
    /// Gets or sets the prompt string displayed to the user.
    /// </summary>

    public string Prompt { get; set; } = "";

    public bool IsRunning => _isRunning;

    /// <summary>
    /// Gets an enumerable collection of all minimal commands registered in the application.
    /// </summary>
    public IEnumerable<IMinimalCommand> MinimalCommands => _commands.Values;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleApplication"/> class with the specified prompt.
    /// </summary>
    /// <param name="prompt">The prompt string to display to the user.</param>
    public ConsoleApplication(string prompt)
    {
        Prompt = prompt;

        Current = this;
    }

    /// <summary>
    /// Adds a minimal command with the specified name, action, and optional description.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="action">The action to execute when the command is invoked.</param>
    /// <param name="description">The description of the command.</param>
    /// <returns>The created <see cref="MinimalCommand"/> instance.</returns>
    public MinimalCommand AddCommand(string name, Action<string> action, string description = "")
    {
        var command = new MinimalCommand(name, description, action);

        _commands.Add(name, command);

        return command;
    }

    /// <summary>
    /// Adds a minimal command with options, specifying the name, action, and optional description.
    /// </summary>
    /// <typeparam name="TOptions">The type of options for the command.</typeparam>
    /// <param name="name">The name of the command.</param>
    /// <param name="action">The action to execute when the command is invoked.</param>
    /// <param name="description">The description of the command.</param>
    /// <returns>The created <see cref="MinimalCommand{TOptions}"/> instance.</returns>
    public MinimalCommand<TOptions> AddCommand<TOptions>(string name, Action<TOptions> action, string description = "")
    {
        var command = new MinimalCommand<TOptions>(name, description, action);

        _commands.Add(name, command);

        return command;
    }

    /// <summary>
    /// Handles the <c>Ctrl+C</c> key press event to prevent application termination.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void HandleCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        if (TerminationMode.HasFlag(TerminationMode.TerminateOnCtrlC)) {
            _isRunning = false;

            e.Cancel = false; // Allow the application to terminate.
        }
        else {
            e.Cancel = true; // Prevent the application from terminating.
            SmartConsole.LogWarning("Application will not terminate on Ctrl+C. Use 'exit' command to quit.");
        }
    }

    /// <summary>
    /// Runs the console application asynchronously, processing user input and executing commands.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RunAsync()
    {
        _isRunning = true;

        Console.CancelKeyPress += HandleCancelKeyPress;

        try {
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
        finally {
            Console.CancelKeyPress -= HandleCancelKeyPress;
        }
    }

    /// <summary>
    /// Executes a user command asynchronously and returns the execution status.
    /// </summary>
    /// <param name="userCommand">The user command string to execute.</param>
    /// <returns>
    /// <c>true</c> if the command executed successfully; otherwise, <c>false</c>.
    /// </returns>
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

    /// <summary>
    /// Creates a new instance of <see cref="ConsoleApplication"/> asynchronously with the specified prompt.
    /// </summary>
    /// <param name="prompt">The prompt string to display to the user. Defaults to "Console".</param>
    /// <returns>A task representing the asynchronous operation, with the created <see cref="ConsoleApplication"/> instance as the result.</returns>
    public static async Task<ConsoleApplication> CreateAsync(string prompt = "Console")
    {
        var path = Application.GetPath("history.json");

        var history = await CommandHistory.CreateAsync(path);

        return new ConsoleApplication(prompt) {
            History = history
        };
    }
}
