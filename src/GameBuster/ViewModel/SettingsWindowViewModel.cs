using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GameBuster.Annotations;
using GameBuster.Controller;
using GameBuster.Model;
using GameBuster.Services;
using HDE.Platform.Wpf;
using Microsoft.Win32;

namespace GameBuster.ViewModel
{
    class SettingsWindowViewModel: DependencyObject, INotifyPropertyChanged
    {
        private readonly GameBusterController _controller = GameBusterController.Controller;

        public SettingsWindowViewModel()
        {
            AlarmSoundFile = _controller.Model.Settings.AlarmSoundFile;
            PlayingTimeDurationHours = _controller.Model.Settings.PlayingTimeDurationHours;
        }

        public static readonly DependencyProperty PlayingTimeDurationHoursProperty = DependencyProperty.Register(
            "PlayingTimeDurationHours", typeof (int), typeof (SettingsWindowViewModel), new PropertyMetadata(default(int)));

        public int PlayingTimeDurationHours
        {
            get { return (int) GetValue(PlayingTimeDurationHoursProperty); }
            set { SetValue(PlayingTimeDurationHoursProperty, value); }
        }

        public static readonly DependencyProperty AlarmSoundFileProperty = DependencyProperty.Register("AlarmSoundFile", typeof (string), typeof (SettingsWindowViewModel), new PropertyMetadata(default(string)));
        public string AlarmSoundFile
        {
            get { return (string) GetValue(AlarmSoundFileProperty); }
            set
            {
                SetValue(AlarmSoundFileProperty, value);
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Commands

        public ICommand BrowseAlarmFileCommand { get; } = new ViewModelActionCommand<SettingsWindowViewModel>(
            vm => vm.BrowseAlarmFile());

        private void BrowseAlarmFile()
        {
            var browseAlarmFileDialog = new OpenFileDialog();
            var supportedFormats = new SoundService().GetSupportedAudioFormats();

            browseAlarmFileDialog.Filter = 
                string.Join(",", 
                    supportedFormats
                        .Select(item=>item.ToUpperInvariant())
                        .ToArray()) + "|" + 
                string.Join(";",
                    supportedFormats
                        .Select( item => $"*.{item}")
                        .ToArray());
            browseAlarmFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Media");
            if (browseAlarmFileDialog.ShowDialog() == true)
            {
                AlarmSoundFile = browseAlarmFileDialog.FileName;
            }
        }

        public ICommand AcceptSettingsCommand { get; } = new ViewModelActionCommand<SettingsWindowViewModel>(
            vm => vm.AcceptSettings());

        private void AcceptSettings()
        {
            _controller.AcceptSettings(new GameBusterSettings(AlarmSoundFile, PlayingTimeDurationHours));

            Application.Current.MainWindow.Close();
            Application.Current.MainWindow = null;
        }

        public ICommand DeclineSettingsCommand { get; } = new ViewModelActionCommand<SettingsWindowViewModel>(
            vm => vm.DeclineSettings());

        private void DeclineSettings()
        {
            Application.Current.MainWindow.Close();
            Application.Current.MainWindow = null;
        }

        #endregion

    }
}
