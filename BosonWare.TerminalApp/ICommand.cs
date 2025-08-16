using JetBrains.Annotations;

namespace BosonWare.TerminalApp;

[PublicAPI]
public interface ICommand
{
    Task Execute(string arguments);
}
