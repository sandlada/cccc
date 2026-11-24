using System;

namespace Cccc.Application
{
    public sealed class InitializeSettingsHandler
    {
        private readonly PreferencesService preferences;
        private readonly StartupRegistration startup;

        public InitializeSettingsHandler(PreferencesService preferences, StartupRegistration startup)
        {
            this.preferences = preferences;
            this.startup = startup;
        }

        public InitializationResult Handle()
        {
            preferences.Load();
            bool initialized = preferences.InitializeFirstRun(out string error);
            StartupState state = null;
            try { state = startup.Read(); }
            catch (Exception) { }

            return new InitializationResult
            {
                Initialized = initialized,
                NeedsWarning = !initialized,
                Preferences = preferences.Current,
                Startup = state,
                Error = error ?? ""
            };
        }
    }
}
