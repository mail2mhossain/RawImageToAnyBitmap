using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia_Test.ViewModels;
using Avalonia_Test.Views;

namespace Avalonia_Test
{
    public partial class App : Application
    {
        public override void Initialize ()
        {
            DataContext = new AppViewModel();
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted ()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                MainWindowViewModel viewModel= new MainWindowViewModel();
                MainWindow window = new MainWindow
                {
                    DataContext = viewModel,
                };
                desktop.MainWindow= window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}