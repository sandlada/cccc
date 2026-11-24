using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using Cccc.Platform.Windows;

namespace Cccc.Adapters.Inbound.Desktop
{
    internal static class TaskbarLayoutDialog
    {
        internal static bool Confirm(bool dark)
        {
            bool highContrast = SystemParameters.HighContrast;
            Brush background = highContrast ? SystemColors.WindowBrush :
                new SolidColorBrush(dark ? Color.FromRgb(43, 43, 43) : Colors.White);
            Brush foreground = highContrast ? SystemColors.WindowTextBrush :
                new SolidColorBrush(dark ? Colors.White : Color.FromRgb(26, 26, 26));
            Window dialog = new Window
            {
                Title = "Cccc",
                Width = 440,
                SizeToContent = SizeToContent.Height,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = background,
                Foreground = foreground,
                FontSize = 14,
                FontFamily = SystemFonts.MessageFontFamily,
                ShowInTaskbar = false
            };
            if (!highContrast)
                dialog.SourceInitialized += delegate
                {
                    int useDarkTitle = dark ? 1 : 0;
                    WindowsNativeMethods.DwmSetWindowAttribute(new WindowInteropHelper(dialog).Handle,
                        WindowsNativeMethods.DwmWindowAttributeUseImmersiveDarkMode, ref useDarkTitle, sizeof(int));
                };
            DockPanel layout = new DockPanel();
            StackPanel content = new StackPanel { Margin = new Thickness(24) };
            content.Children.Add(new TextBlock
            {
                Text = "试用任务栏单行",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Foreground = foreground
            });
            content.Children.Add(new TextBlock
            {
                Text = "这项功能仍在试验中。任务栏空间不足或自动隐藏时，提示可能遮住任务栏上的图标，也可能显示在任务栏上方。",
                Margin = new Thickness(0, 12, 0, 0),
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22,
                Foreground = foreground
            });
            Grid buttons = new Grid { Margin = new Thickness(24, 16, 24, 16) };
            buttons.ColumnDefinitions.Add(new ColumnDefinition());
            buttons.ColumnDefinitions.Add(new ColumnDefinition());
            Button confirm = new Button { Content = "切换样式", MinHeight = 34, Margin = new Thickness(0, 0, 4, 0) };
            Button cancel = new Button { Content = "取消", MinHeight = 34, Margin = new Thickness(4, 0, 0, 0), IsCancel = true };
            Border footer = new Border { Child = buttons };
            if (!highContrast)
            {
                footer.Background = new SolidColorBrush(dark ? Color.FromRgb(32, 32, 32) : Color.FromRgb(246, 246, 246));
                footer.BorderBrush = new SolidColorBrush(dark ? Color.FromRgb(29, 29, 29) : Color.FromRgb(226, 226, 226));
                footer.BorderThickness = new Thickness(0, 1, 0, 0);
                ControlTemplate template = (ControlTemplate)XamlReader.Parse(
                    @"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" TargetType=""Button"">
                        <Border Name=""Surface"" CornerRadius=""4"" Background=""{TemplateBinding Background}""
                                BorderBrush=""{TemplateBinding BorderBrush}"" BorderThickness=""{TemplateBinding BorderThickness}"">
                            <ContentPresenter HorizontalAlignment=""Center"" VerticalAlignment=""Center"" RecognizesAccessKey=""True""/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property=""IsMouseOver"" Value=""True""><Setter TargetName=""Surface"" Property=""Opacity"" Value=""0.88""/></Trigger>
                            <Trigger Property=""IsPressed"" Value=""True""><Setter TargetName=""Surface"" Property=""Opacity"" Value=""0.75""/></Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>");
                confirm.Template = cancel.Template = template;
                confirm.Background = new SolidColorBrush(dark ? Color.FromRgb(96, 205, 255) : Color.FromRgb(0, 95, 184));
                confirm.Foreground = dark ? Brushes.Black : Brushes.White;
                confirm.BorderThickness = new Thickness(0);
                cancel.Background = new SolidColorBrush(dark ? Color.FromRgb(45, 45, 45) : Colors.White);
                cancel.BorderBrush = new SolidColorBrush(dark ? Color.FromRgb(62, 62, 62) : Color.FromRgb(209, 209, 209));
                cancel.Foreground = foreground;
            }
            confirm.Click += delegate { dialog.DialogResult = true; };
            dialog.Loaded += delegate { cancel.Focus(); };
            Grid.SetColumn(cancel, 1);
            buttons.Children.Add(confirm);
            buttons.Children.Add(cancel);
            DockPanel.SetDock(footer, Dock.Bottom);
            layout.Children.Add(footer);
            layout.Children.Add(content);
            dialog.Content = layout;
            return dialog.ShowDialog() == true;
        }
    }
}
