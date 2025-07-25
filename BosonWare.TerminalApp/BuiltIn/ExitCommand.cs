﻿using BosonWare.TUI;

namespace BosonWare.TerminalApp.BuiltIn;

[Command("exit", Aliases = ["shutdown"], Description = "Shutdown application.")]
public sealed class ExitCommand : ICommand
{
    public async Task Execute(string arguments)
    {
        SmartConsole.WriteLine("Exiting application...", ConsoleColor.Yellow);

        ConsoleApplication.Current.Exit();
    }
}
