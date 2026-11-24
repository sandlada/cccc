using System;

namespace Cccc.Application
{
    public sealed class PreferencesService
    {
        private readonly IPreferencesStore currentStore;
        private readonly IPreferencesStore legacyStore;

        public PreferencesService(IPreferencesStore currentStore, IPreferencesStore legacyStore)
        {
            ArgumentNullException.ThrowIfNull(currentStore);
            ArgumentNullException.ThrowIfNull(legacyStore);
            this.currentStore = currentStore;
            this.legacyStore = legacyStore;
            Current = new();
        }

        public PreferencesModel Current { get; private set; }
        public PreferencesLoadState State { get; private set; }
        public PreferencesLoadState LegacyState { get; private set; }
        public string LastError { get; private set; }

        public void Load()
        {
            PreferencesLoadResult current = currentStore.Load();
            PreferencesLoadResult legacy = legacyStore.Load();
            State = current.State;
            LastError = current.Error;
            Current = current.Model ?? new();
            LegacyState = legacy.State;

            if (State == PreferencesLoadState.Missing && legacy.State == PreferencesLoadState.Loaded)
            {
                Current.ShowPreview = legacy.Model.ShowPreview;
                Current.UseMaterial = legacy.Model.UseMaterial;
                Current.Layout = LayoutMode.Card;
            }
        }

        public bool Save(out string error)
        {
            PreferencesSaveResult result = currentStore.Save(Current);
            error = result.Success ? null : result.Error;
            if (result.Success) State = PreferencesLoadState.Loaded;
            return result.Success;
        }

        public bool InitializeFirstRun(out string error)
        {
            error = null;
            if (State == PreferencesLoadState.ReadError) return false;
            if (State != PreferencesLoadState.Missing) return true;
            if (LegacyState == PreferencesLoadState.ReadError) return false;
            if (LegacyState != PreferencesLoadState.Missing) return true;

            // First run only persists defaults. Startup registration stays an
            // explicit user action so this fork never hijacks the upstream
            // CCCC entry or starts silently at login.
            return Save(out error);
        }
    }
}
