namespace BosonWare.TerminalApp;

[Flags]
public enum TerminationMode
{
    /// <summary>
    /// The application will terminate when the user presses <c>Ctrl+C</c>.
    /// </summary>
    TerminateOnCtrlC = 0x1,

    /// <summary>
    /// The application will not terminate on <c>Ctrl+C</c> and will continue running.
    /// </summary>
    IgnoreCtrlC = 0x2,
}
