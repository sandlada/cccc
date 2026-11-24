using System;
using System.Windows.Threading;
using Cccc.Application;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    public sealed class ToastPresenter : IFeedbackPresenter, IDisposable
    {
        private readonly Dispatcher dispatcher;
        private PreferencesModel preferences;
        private ToastWindow window;
        private bool disposed;

        public ToastPresenter(PreferencesModel preferences, Dispatcher dispatcher)
        {
            if (preferences == null) throw new ArgumentNullException("preferences");
            this.preferences = Clone(preferences);
            this.dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
            ApplyProfile();
            window = new ToastWindow(this.preferences.UseMaterial);
            window.ApplyPersonalization(this.preferences);
            if (ToastProfile.Compact) TaskbarPlacement.RequestRefresh();
        }

        public bool IsDark { get { return Themes.Current.Dark; } }

        public string TaskbarStatus { get { return TaskbarPlacement.LastDecision; } }

        public void CopyPressed(CopyFeedbackSignal signal)
        {
            Post(delegate
            {
                if (window == null) return;
                window.CopyPressed(signal.Count, new IntPtr(signal.Source.Handle),
                    signal.ShowPreview, signal.CurrentTimeMilliseconds, signal.CountText);
            });
        }

        public void Present(FeedbackView feedback)
        {
            if (feedback == null) return;
            Post(delegate
            {
                if (window != null) window.Present(feedback, preferences.ShowPreview);
            });
        }

        public void Hide()
        {
            Post(delegate
            {
                if (window != null) window.HideAndClear();
            });
        }

        public void UpdatePreferences(PreferencesModel model)
        {
            if (model == null) return;
            Post(delegate
            {
                preferences = Clone(model);
                ApplyProfile();
                if (window != null)
                {
                    window.SetMaterialEnabled(preferences.UseMaterial);
                    window.ApplyPersonalization(preferences);
                    window.RefreshAppearance();
                }
            });
        }

        public void ReplaceLayout(LayoutMode layout, bool material)
        {
            Post(delegate
            {
                if (window != null)
                {
                    window.HideAndClear();
                    window.Close();
                    window = null;
                }
                preferences.Layout = layout;
                preferences.UseMaterial = material;
                ApplyProfile();
                Themes.MaterialAvailable = false;
                Themes.Refresh();
                window = new ToastWindow(material);
                window.ApplyPersonalization(preferences);
                if (ToastProfile.Compact) TaskbarPlacement.RequestRefresh();
            });
        }

        public void RefreshAppearance()
        {
            Post(delegate
            {
                Themes.Refresh();
                if (window != null) window.RefreshAppearance();
            });
        }

        public void RenderSample(string path)
        {
            if (window == null) window = new ToastWindow(preferences.UseMaterial);
            window.RenderSample(path);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            if (window != null)
            {
                if (dispatcher.CheckAccess())
                {
                    window.Close();
                    window = null;
                }
                else
                {
                    dispatcher.BeginInvoke(new Action(delegate
                    {
                        if (window != null) { window.Close(); window = null; }
                    }));
                }
            }
        }

        private void ApplyProfile()
        {
            ToastProfile.SetCompact(preferences.Layout == LayoutMode.Taskbar);
        }

        private void Post(Action action)
        {
            if (dispatcher.CheckAccess()) action();
            else dispatcher.BeginInvoke(action);
        }

        private static PreferencesModel Clone(PreferencesModel value)
        {
            return new PreferencesModel
            {
                ShowPreview = value.ShowPreview,
                UseMaterial = value.UseMaterial,
                Layout = value.Layout,
                DisplayMode = value.DisplayMode,
                CornerRadius = value.CornerRadius,
                FullCornerRadius = value.FullCornerRadius,
                Padding = value.Padding,
                ShowBorder = value.ShowBorder,
                ShowShadow = value.ShowShadow
            };
        }
    }
}
