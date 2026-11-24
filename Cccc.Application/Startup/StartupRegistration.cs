using System;
using System.IO;

namespace Cccc.Application
{
    public sealed class StartupRegistration
    {
        private readonly IStartupStore store;
        private readonly string command;

        public StartupRegistration(IStartupStore store, string executable)
        {
            ArgumentNullException.ThrowIfNull(store);
            this.store = store;
            command = CommandFor(executable);
        }

        public static string CommandFor(string executable)
        {
            if (string.IsNullOrWhiteSpace(executable) || !Path.IsPathRooted(executable) ||
                executable.IndexOfAny(['"', '\r', '\n', '\0']) >= 0)
                throw new ArgumentException("An absolute executable path is required.");
            return $"\"{Path.GetFullPath(executable)}\"";
        }

        public StartupState Read()
        {
            string value = store.Read();
            return new()
            {
                Exists = value is not null,
                Enabled = !string.IsNullOrWhiteSpace(value),
                CurrentLocation = string.Equals(value, command, StringComparison.OrdinalIgnoreCase)
            };
        }

        public StartupState SetEnabled(bool enabled)
        {
            if (enabled)
            {
                // Run values have a documented 260-character command-line limit.
                if (command.Length > 260) throw new IOException("The startup path is too long.");
                store.Write(command);
            }
            else store.Delete();
            StartupState result = Read();
            if (result.Enabled != enabled || (enabled && !result.CurrentLocation))
                throw new IOException("The startup setting was not saved.");
            return result;
        }
    }
}
