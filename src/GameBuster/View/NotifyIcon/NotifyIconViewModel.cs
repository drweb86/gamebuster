using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GameBuster.Annotations;
using GameBuster.Controller;

namespace GameBuster.View.NotifyIcon
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel: DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty RemainingTimeProperty = DependencyProperty.Register(
            "RemainingTime", typeof (string), typeof (NotifyIconViewModel), new PropertyMetadata(default(string)));

        public string RemainingTime
        {
            get { return (string) GetValue(RemainingTimeProperty); }
            set
            {
                SetValue(RemainingTimeProperty, value); 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => true,
                    CommandAction = () =>
                    {
                        if (Application.Current.MainWindow == null)
                            Application.Current.MainWindow = new SettingsWindow();

                        if (!Application.Current.MainWindow.IsVisible)
                            Application.Current.MainWindow.Show();
                    }
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => Application.Current.MainWindow.Close(),
                    CanExecuteFunc = () => Application.Current.MainWindow != null
                };
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RefreshRemainingTime()
        {
            string text;
            var remainingTime = GameBusterController.Controller.GetRemainingTime();
            if (remainingTime <= new TimeSpan(0, 0, 0, 0))
                text = "Time for gaming finished.";
            else
                text = $"{remainingTime.TotalMinutes} minute(s) remained";

            RemainingTime = text;
        }
    }


    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
