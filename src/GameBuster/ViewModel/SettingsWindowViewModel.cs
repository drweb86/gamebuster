using System;
using System.Collections.Generic;
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
            RefreshProcesses();
            SetKnownGames(_controller.Model.Settings.KnownGames); //TODO:
        }

        private void RefreshProcesses()
        {
            ProcessNames = _controller
                .GetProcessNames()
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .Select(item=>new CheckItem(item, false))
                .ToArray();
        }

        public static readonly DependencyProperty GameProcessNamesProperty = DependencyProperty.Register(
            "GameProcessNames", typeof (List<CheckItem>), typeof (SettingsWindowViewModel), new PropertyMetadata(default(List<CheckItem>)));
        
        public List<CheckItem> GameProcessNames
        {
            get { return (List<CheckItem>) GetValue(GameProcessNamesProperty); }
            set
            {
                SetValue(GameProcessNamesProperty, value); 
                OnPropertyChanged();
            }
        }

        public ICommand RefreshProcessNamesCommand { get; } = new ViewModelActionCommand<SettingsWindowViewModel>(vm=>vm.RefreshProcesses());
        public ICommand AddSelectedProcessesCommand { get; } = new ViewModelActionCommand<SettingsWindowViewModel>(vm => vm.AddSelectedProcesses());
        public ICommand RemoveSelectedGamesCommand { get; } = new ViewModelActionCommand<SettingsWindowViewModel>(vm => vm.RemoveSelectedGames());

        private void RemoveSelectedGames()
        {
            GameProcessNames = GameProcessNames
                .Where(item => !item.IsChecked)
                .ToList();
        }

        public ICommand AddProcessNameCommand { get; } = new ViewModelActionCommand<SettingsWindowViewModel>(vm => vm.AddProcessName());

        public static readonly DependencyProperty ProcessNameProperty = DependencyProperty.Register(
            "ProcessName", typeof (string), typeof (SettingsWindowViewModel), new PropertyMetadata(default(string)));

        public string ProcessName
        {
            get { return (string) GetValue(ProcessNameProperty); }
            set { SetValue(ProcessNameProperty, value); }
        }

        private void AddProcessName()
        {
            var gameProcessNames = GameProcessNames
                .Select(item => item.Title)
                .ToList();

            if (!gameProcessNames.Contains(ProcessName))
            {
                gameProcessNames.Add(ProcessName);
            }

            SetKnownGames(gameProcessNames);
        }

        private void AddSelectedProcesses()
        {
            var gameProcessNames = GameProcessNames
                .Select(item => item.Title)
                .ToList();

            var itemsToAdd = ProcessNames
                .Where(item => item.IsChecked)
                .ToArray();

            foreach (var itemToAdd in itemsToAdd)
            {
                itemToAdd.IsChecked = false;

                if (!gameProcessNames.Contains(itemToAdd.Title))
                {
                    gameProcessNames.Add(itemToAdd.Title);
                }
            }

            SetKnownGames(gameProcessNames);
        }

        private void SetKnownGames(List<string> gameProcessNames)
        {
            GameProcessNames = gameProcessNames
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .Select(item => new CheckItem(item, false))
                .ToList();
        }

        public static readonly DependencyProperty ProcessNamesProperty = DependencyProperty.Register(
            "ProcessNames", typeof (IEnumerable<CheckItem>), typeof (SettingsWindowViewModel), new PropertyMetadata(default(IEnumerable<CheckItem>)));

        public IEnumerable<CheckItem> ProcessNames
        {
            get { return (IEnumerable<CheckItem>) GetValue(ProcessNamesProperty); }
            set
            {
                SetValue(ProcessNamesProperty, value);
                OnPropertyChanged();
            }
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
            var supportedFormats = new SoundService(_controller.Log).GetSupportedAudioFormats();

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
            var knownGames = GameProcessNames
                .Select(item => item.Title)
                .ToList();

            _controller.AcceptSettings(new GameBusterSettings(AlarmSoundFile, PlayingTimeDurationHours, knownGames));

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
