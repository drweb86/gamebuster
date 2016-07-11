using System;
using System.Diagnostics;
using HDE.Platform.Logging;

namespace GameBuster.Services
{
    class GameWatcherService: PerMinuteJobService
    {
        #region Constants

        private const int SleepStartsAtHoursAM = 1;
        private const int SleepFinishesAtHoursAM = 7;

        #endregion

        #region Fields

        private DateTime _lastAddTimePeriod = DateTime.MinValue;
        private TimeSpan _playingTimeRemained;
        
        #endregion

        #region Constructors

        public GameWatcherService(ILog log):base(log)
        {
        }

        #endregion

        protected override void DoJobQuant()
        {
            ExtendPlayingTime();

            if (!GameIsRunning())
                return;

            if (IsSleepingTime)
            {
                PlaySound();
            }
            else if (_playingTimeRemained <= new TimeSpan(0, 0, 0, 0))
            {
                PlaySound();
            }
            else
            {
                _playingTimeRemained -= _refreshProcessesTimeout;
                _log.Info($"{_playingTimeRemained.TotalMinutes} minute(s) of playing remained.");
            }

            if (TimeToKill())
            {
                KillRunningGames();
            }
        }

        private void KillRunningGames()
        {
            _log.Info("Killing games");
            var processHelperService = new ProcessHelperService(_log);
            var userProcessNames = processHelperService.GetCurrentUserProcessNames(true, true);
            foreach (var userProcess in userProcessNames)
            {
                if (_settings.IsKnown(userProcess))
                {
                    foreach (var process in Process.GetProcessesByName(userProcess))
                    {
                        _log.Error($"Killing {process.ProcessName}...");
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception e)
                        {
                            _log.Error(e);
                        }
                    }
                }
            }
        }

        private bool TimeToKill()
        {
            return DateTime.Now.Hour >= _settings.BeginKillGameIntervalHour &&
                   DateTime.Now.Hour <= _settings.EndKillGameIntervalHour;
        }

        private bool GameIsRunning()
        {
            var processHelperService = new ProcessHelperService(_log);
            var userProcessNames = processHelperService.GetCurrentUserProcessNames(true, true);
            foreach (var userProcess in userProcessNames)
            {
                if (_settings.IsKnown(userProcess))
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

        public TimeSpan PlayingTimeRemained => _playingTimeRemained;
        public bool IsSleepingTime => DateTime.Now.Hour >= SleepStartsAtHoursAM && DateTime.Now.Hour <= SleepFinishesAtHoursAM;

        public void Extend(TimeSpan playingTime)
        {
            _playingTimeRemained += playingTime;
        }
    }
}
