namespace BosonWare.TerminalApp.BuiltIn;

[Command("clear", Aliases = ["cls"], Description = "Clears the terminal")]
public sealed class ClearCommand : ICommand
{
    public Task Execute(string arguments)
    {
        Console.Clear();

        return Task.CompletedTask;
    }
}
