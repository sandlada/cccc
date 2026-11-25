using Cccc.Domain.Preferences;

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
            if (command.DisplayMode is { } displayMode) model.DisplayMode = displayMode;
            if (command.CornerRadius is { } cornerRadius) model.CornerRadius = ToastAppearancePolicy.ClampCornerRadius(cornerRadius);
            if (command.FullCornerRadius is { } fullCornerRadius) model.FullCornerRadius = fullCornerRadius;
            if (command.CounterCornerRadius is { } counterCornerRadius) model.CounterCornerRadius = ToastAppearancePolicy.ClampCounterCornerRadius(counterCornerRadius);
            if (command.FullCounterCornerRadius is { } fullCounterCornerRadius) model.FullCounterCornerRadius = fullCounterCornerRadius;
            if (command.Padding is { } padding) model.Padding = ToastAppearancePolicy.ClampPadding(padding);
            if (command.ShowBorder is { } showBorder) model.ShowBorder = showBorder;
            if (command.ShowShadow is { } showShadow) model.ShowShadow = showShadow;
            if (command.FontFamily is { } fontFamily) model.FontFamily = ToastAppearancePolicy.NormalizeFontFamily(fontFamily);

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
