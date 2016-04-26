using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly int _sessionId;
        private GameBusterSettings _settings;

        #endregion

        #region Constructors

        public GameWatcherService(ILog log)
        {
            _log = log;
            _sessionId = Process.GetCurrentProcess().SessionId;
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
            
            _log.Debug($"Starting game watcher service (session id: {_sessionId}, refresh timeout: {_refreshProcessesTimeout.TotalMinutes} minute(s), time to play: {settings.PlayingTimeDurationHours} minute(s))...");
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
            var userProcessNames = GetCurrentUserProcessNamees();
            const string secretProcessName = "gta5";
            var result = userProcessNames.Any(processName => string.Compare(
                processName, secretProcessName, StringComparison.OrdinalIgnoreCase) == 0);

            _log.Info(result ? "Game is running" : "Game is not running");

            return result;
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
            _log.Info("Plaing sound...");

            var soundFile = (!string.IsNullOrWhiteSpace(_settings.AlarmSoundFile) && File.Exists(_settings.AlarmSoundFile)) ?
                _settings.AlarmSoundFile:
                FindSystemRandomSound();

            _log.Debug($"Sound file: {soundFile}");

            var soundService = new SoundService();
            soundService.Play(soundFile);

        }

        private string FindSystemRandomSound()
        {
            var soundsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Media");
            if (!Directory.Exists(soundsFolder))
                throw new ApplicationException($"Can't locate directory {soundsFolder}");

            var allWavFiles = Directory.GetFiles(soundsFolder, "Alarm*.wav").ToList();
            allWavFiles.AddRange(Directory.GetFiles(soundsFolder, "Ring*.wav"));
            allWavFiles.AddRange(Directory.GetFiles(soundsFolder, "tada.wav"));

            if (allWavFiles.Count == 0)
                throw new ApplicationException($"Can't locate Alarm*.wav/Ring*.wav/tada.wav music files in {soundsFolder}.");

            var random = new Random();
            return allWavFiles[random.Next(allWavFiles.Count)];
        }

        private IEnumerable<string> GetCurrentUserProcessNamees()
        {
            _log.Info("Populating list of processes:");
            var allProcesses = Process.GetProcesses();

            var currentUserProcessNames = new List<string>();

            foreach (var process in allProcesses)
            {
                try
                {
                    if (process.SessionId == _sessionId)
                    {
                        currentUserProcessNames.Add(process.ProcessName);
                        _log.Info($"- {process.ProcessName}");
                    }
                }
                catch (Exception e)
                {
                    _log.Error($"{nameof(GameWatcherService)}: Failed to load details for process {process.ProcessName}.");
                    _log.Error(e);
                }
            }

            return currentUserProcessNames;
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

        public TimeSpan GetRemainingTime()
        {
            return _playingTimeRemained;
        }

        public void AddMoreTime(TimeSpan timeSpan)
        {
            _playingTimeRemained += timeSpan;
        }
    }
}
