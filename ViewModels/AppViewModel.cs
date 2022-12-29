using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Platform;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia_Test.Views;

namespace Avalonia_Test.ViewModels
{
    public class AppViewModel: ViewModelBase
    {
        #region Command(s)
        public ReactiveCommand<Unit, Unit>? ShowLog { get; set; }
        public ReactiveCommand<Unit, Unit>? ExitCommand { get; set; }
        public ReactiveCommand<Unit, Unit>? ShowConfigWindow { get; set; }
        #endregion

        #region Constructor(s)
        public AppViewModel ()
        {
            ExitCommand = ReactiveCommand.Create(ExitApp);
            ShowLog = ReactiveCommand.CreateFromTask(ShowLogFileAsync);
            ShowConfigWindow = ReactiveCommand.Create(ShowSettingsWindow);
        }
        #endregion

        #region Command Action(s)
        private void ShowSettingsWindow ()
        {
            MainWindowViewModel viewModel = new MainWindowViewModel();
            
            MainWindow settingWindow =
                new MainWindow
                {
                    Title = "Device Config Screen",
                    Width = 300,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    DataContext = viewModel,
                };
            settingWindow.Show();
        }
        private async Task ShowLogFileAsync ()
        {
            
        }
        private void ExitApp ()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.TryShutdown();
            }
        }

        #endregion
    }
}
