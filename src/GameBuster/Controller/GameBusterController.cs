using System;
using System.Collections.Generic;
using GameBuster.Model;
using GameBuster.Services;
using HDE.Platform.AspectOrientedFramework;
using HDE.Platform.Logging;
using HDE.Platform.Services;

namespace GameBuster.Controller
{
    class GameBusterController : BaseController<GameBusterModel>
    {
        private readonly GameWatcherService _gameWatcherService;

        private readonly ApplicationSingleInstancePerUser _singleInstance = new ApplicationSingleInstancePerUser(
#if DEBUG
            "GabeBuster-DEBUG"
#else
            "GabeBuster"
#endif
            );

        protected override ILog CreateOpenLog()
        {
            var fileLog = new SimpleFileLog(FileHelper.LogsFolder);
            fileLog.Open();
            return fileLog;
        }

#region Singleton

        private GameBusterController()
        {
            LoadSettings();
            _gameWatcherService = new GameWatcherService(Log);

            StartService();

            if (!_singleInstance.FirstInstance)
            {
                Log.Warning("Copy of already running application was launched. Exiting.");
                Environment.Exit(1);
            }
        }
        private static GameBusterController _instance;
        public static GameBusterController Controller
        {
            get
            {
                if (_instance == null)
                    _instance = new GameBusterController();

                return _instance;
            }
        }

#endregion

        public override void Dispose()
        {
            StopService();
            _singleInstance.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Starts or restarts service.
        /// </summary>
        private void StartService()
        {
            _gameWatcherService.Start(Model.Settings);
        }

        private void StopService()
        {
            _gameWatcherService.Dispose();
        }

        private void LoadSettings()
        {
            Model.Settings = new SettingsService<GameBusterSettings>(Log)
                .Load();

            // Support for old versions
            if (!GameBusterSettings.IsIntervalHourValid(Model.Settings.BeginKillGameIntervalHour))
                Model.Settings.BeginKillGameIntervalHour = GameBusterSettings.DefaultKillGameIntervalBeginHour;
            if (!GameBusterSettings.IsIntervalHourValid(Model.Settings.EndKillGameIntervalHour))
                Model.Settings.EndKillGameIntervalHour = GameBusterSettings.DefaultKillGameIntervalEndHour;
            if (Model.Settings.BeginKillGameIntervalHour > Model.Settings.EndKillGameIntervalHour)
                Model.Settings.EndKillGameIntervalHour = Model.Settings.BeginKillGameIntervalHour;
        }

        public void AcceptSettings(GameBusterSettings gameBusterSettings)
        {
            Model.Settings = gameBusterSettings;
            new SettingsService<GameBusterSettings>(Log)
                .Save(Model.Settings);
            StartService();
        }

        public TimeSpan PlayingTimeRemained => _gameWatcherService.PlayingTimeRemained;
        public bool IsSleepingTime => _gameWatcherService.IsSleepingTime;

        public void AddMoreTime(TimeSpan timeSpan)
        {
            _gameWatcherService.Extend(timeSpan);
        }

        public IEnumerable<string> GetProcessNames()
        {
            return new ProcessHelperService(Log).GetCurrentUserProcessNames(true, true);
        }
    }
}
