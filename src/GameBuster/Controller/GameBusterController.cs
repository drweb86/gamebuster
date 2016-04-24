using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        public void AcceptSettings(GameBusterSettings gameBusterSettings)
        {
            Model.Settings = gameBusterSettings;
            new SettingsService<GameBusterSettings>(Log)
                .Save(Model.Settings);
            StartService();
        }
    }
}
