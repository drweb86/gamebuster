using System;
using System.Linq;
using System.Threading;
using GameBuster.Model;
using HDE.Platform.Logging;

namespace GameBuster.Services
{
    class GameWatcherService: IDisposable
    {
        #region Fields

        private DateTime _lastAddTimePeriod = DateTime.MinValue;
        private readonly TimeSpan _refreshProcessesTimeout = new TimeSpan(0, 0, 1, 0);
        private readonly ILog _log;
        private Thread _watchThread;
        private TimeSpan _playingTimeRemained;
        private GameBusterSettings _settings;

        #endregion

        #region Constructors

        public GameWatcherService(ILog log)
        {
            _log = log;
        }

        #endregion

        /// <summary>
        /// Starts or restarts the game watch service...
        /// </summary>
        /// <param name="settings"></param>
        public void Start(GameBusterSettings settings)
        {
            Dispose();

            _settings = settings;
            
            _log.Debug($"Starting game watcher service (refresh timeout: {_refreshProcessesTimeout.TotalMinutes} minute(s), time to play: {settings.PlayingTimeDurationHours} minute(s))...");
            _log.Debug($"Settings: alarm sound file: {settings.AlarmSoundFile}; playing time duration, hours: {settings.PlayingTimeDurationHours}");

            _watchThread = new Thread(OnWatchGames);
            _watchThread.Start();
        }

        private void OnWatchGames()
        {
            while (true)
            {
                ExtendPlayingTime();

                Thread.Sleep(_refreshProcessesTimeout);

                try
                {
                    if (!GameIsRunning())
                        continue;

                    if (_playingTimeRemained <= new TimeSpan(0, 0, 0, 0))
                    {
                        PlaySound();
                    }
                    else
                    {
                        _playingTimeRemained -= _refreshProcessesTimeout;
                        _log.Info($"{_playingTimeRemained.TotalMinutes} minute(s) of playing remained.");
                    }
                }
                catch (Exception e)
                {
                    _log.Error($"{nameof(GameWatcherService)}: Failed to watch games.");
                    _log.Error(e);
                }
            }
        }

        private bool GameIsRunning()
        {
            var processHelperService = new ProcessHelperService(_log);
            var userProcessNames = processHelperService.GetCurrentUserProcessNames();
            foreach (var userProcess in userProcessNames)
            {
                if (_settings.KnownGames.Contains(userProcess))
                {
                    _log.Info($"{userProcess} is running!");
                    return true;
                }
            }

            _log.Info("Game is not running");
            return false;
        }

        /// <summary>
        /// Extends playing time each 24 hours on settings.PlayingTimeDurationHours
        /// </summary>
        private void ExtendPlayingTime()
        {
            var currentTime = DateTime.Now;
            if (currentTime - _lastAddTimePeriod > new TimeSpan(1, 0, 0, 0))
            {
                _lastAddTimePeriod = currentTime;
                _playingTimeRemained = new TimeSpan(0, _settings.PlayingTimeDurationHours, 0, 0);
            }
        }

        private void PlaySound()
        {
            var soundService = new SoundService(_log);
            var soundFile = soundService.FindAlarmSound(_settings.AlarmSoundFile);
            soundService.Play(soundFile);
        }

        public void Dispose()
        {
            if (_watchThread != null)
            {
                _log.Debug("Stopping game watcher service...");
                _watchThread.Abort();
                _watchThread = null;
            }
        }

        public TimeSpan PlayingTimeRemained => _playingTimeRemained;

        public void Extend(TimeSpan playingTime)
        {
            _playingTimeRemained += playingTime;
        }
    }
}
