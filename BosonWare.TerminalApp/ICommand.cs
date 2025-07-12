namespace BosonWare.TerminalApp;

public interface ICommand
{
    Task Execute(string arguments);
}
