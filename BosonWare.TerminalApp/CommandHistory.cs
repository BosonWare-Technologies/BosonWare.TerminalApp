using BosonWare.Persistence;

namespace BosonWare.TerminalApp;

public sealed class CommandHistory
{
    public required PersistentList<string> History { get; init; }

    public int Count => History.Count;

    public async Task AddEntry(string command)
    {
        int index = History.IndexOf(command);

        if (index > 0) {
            History.RemoveAt(index);

            await History.AddAsync(command);

            return;
        }

        if (History.Count > 1000) {
            return;
        }

        await History.AddAsync(command);
    }

    public static async Task<CommandHistory> CreateAsync(string path)
    {
        var history = await PersistentList<string>.CreateAsync(path, (location) => {
            return new PersistentList<string>(location);
        });

        return new CommandHistory() {
            History = history
        };
    }
}