using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HDE.Platform.Logging;

namespace GameBuster.Services
{
    class GameWatcherService: IDisposable
    {
        private readonly TimeSpan _refreshProcessesTimeout;
        private readonly ILog _log;
        private Thread _watchThread;
        private TimeSpan _timeToPlayRemainedMinutes;
        private readonly int _sessionId;

        public GameWatcherService(ILog log)
        {
            _refreshProcessesTimeout = new TimeSpan(0, 0, 1, 0);
            _log = log;
            _timeToPlayRemainedMinutes = new TimeSpan(0,5,0,0);
            _sessionId = Process.GetCurrentProcess().SessionId;
            _log.Debug($"Starting game watcher service (session id: {_sessionId}, refresh timeout: {_refreshProcessesTimeout.TotalMinutes} minute(s), time to play: {_timeToPlayRemainedMinutes.TotalMinutes} minute(s))...");
        }

        public void Start()
        {
            Dispose();

            _watchThread = new Thread(OnWatchGames);
            _watchThread.Start();
        }

        private void OnWatchGames()
        {
            while (true)
            {
                Thread.Sleep(_refreshProcessesTimeout);

                try
                {
                    var userProcessNames = GetCurrentUserProcessNamees();
                    const string secretProcessName = "gta5";
                    if (userProcessNames.Any(processName => string.Compare(
                        processName, secretProcessName, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        if (_timeToPlayRemainedMinutes <= new TimeSpan(0, 0, 0, 0))
                        {
                            PlaySound();
                        }
                        else
                        {
                            _timeToPlayRemainedMinutes -= _refreshProcessesTimeout;
                            _log.Info($"{_timeToPlayRemainedMinutes.TotalMinutes} minute(s) of plaing remained ({secretProcessName} was detected).");
                        }
                    }
                    else
                    {
                        _log.Debug("Process was not detected.");
                    }
                }
                catch (Exception e)
                {
                    _log.Error($"{nameof(GameWatcherService)}: Failed to watch games.");
                    _log.Error(e);
                }
            }
        }

        private void PlaySound()
        {
            _log.Info("Plaing sound...");
            using (var simpleSound = new SoundPlayer(@"c:\Windows\Media\chimes.wav"))
            {
                simpleSound.PlaySync();
            }
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
    }
}
