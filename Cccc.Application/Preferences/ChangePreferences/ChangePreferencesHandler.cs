namespace Cccc.Application
{
    public sealed class ChangePreferencesHandler
    {
        private readonly PreferencesService preferences;

        public ChangePreferencesHandler(PreferencesService preferences)
        {
            this.preferences = preferences;
        }

        public ChangePreferencesResult Handle(ChangePreferencesCommand command)
        {
            PreferencesModel model = preferences.Current;
            if (command.ShowPreview is { } showPreview) model.ShowPreview = showPreview;
            if (command.UseMaterial is { } useMaterial) model.UseMaterial = useMaterial;
            if (command.Layout is { } layout) model.Layout = layout;

            bool saved = preferences.Save(out string error);
            return new()
            {
                Preferences = model,
                Success = saved,
                Error = error
            };
        }
    }
}
