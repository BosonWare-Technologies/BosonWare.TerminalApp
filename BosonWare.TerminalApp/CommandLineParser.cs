using System.Runtime.CompilerServices;
using System.Text;

namespace BosonWare.TerminalApp;

public static class CommandLineParser
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (string CommandName, string Args) Parse(string command)
    {
        command = command.Trim();

        var num = command.IndexOf(' ');

        if (num > 0) {
            var name = command[..num];
            var arguments = command[(num + 1)..];

            return (name, arguments);
        }

        return (command, "");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] SplitCommandLine(string commandLine)
    {
        List<string> parts = [];
        var partBuilder = new StringBuilder();
        var inQuotes = false;
        char? quoteChar = null;

        for (var i = 0; i < commandLine.Length; i++) {
            var c = commandLine[i];

            if ((c == '"' || c == '\'') && (i == 0 || commandLine[i - 1] != '\\')) {
                if (inQuotes && c == quoteChar) {
                    inQuotes = false;
                    quoteChar = null;
                }
                else if (!inQuotes) {
                    inQuotes = true;
                    quoteChar = c;
                }
                else {
                    partBuilder.Append(c);
                }
            }
            else if (c == ' ' && !inQuotes) {
                if (partBuilder.Length > 0) {
                    parts.Add(partBuilder.ToString());

                    partBuilder.Clear();
                }
            }
            else {
                partBuilder.Append(c);
            }
        }

        if (partBuilder.Length > 0) {
            parts.Add(partBuilder.ToString());
        }

        return [.. parts];
    }
}
