using Avalonia;
using Avalonia.Markup.Xaml;

namespace GitBook1
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new GitBook.Views.MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}