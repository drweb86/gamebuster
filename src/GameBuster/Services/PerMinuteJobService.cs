using System;
using System.Threading;
using GameBuster.Model;
using HDE.Platform.Logging;

namespace GameBuster.Services
{
    /// <summary>
    /// Performs job quant method each timeout. Handles errors.
    /// </summary>
    abstract class PerMinuteJobService : IDisposable
    {
        private Thread _watchThread;
        protected readonly TimeSpan _refreshProcessesTimeout = new TimeSpan(0, 0, 1, 0);
        protected readonly ILog _log;
        protected GameBusterSettings _settings;

        protected PerMinuteJobService(ILog log)
        {
            _log = log;
        }

        public void Start(GameBusterSettings settings)
        {
            Dispose();

            _settings = settings;

            _log.Debug($"Starting {GetType().Name} service (refresh timeout: {_refreshProcessesTimeout.TotalMinutes} minute(s), time to play: {settings.PlayingTimeDurationHours} minute(s))...");
            _log.Debug($"Settings: alarm sound file: {settings.AlarmSoundFile}; playing time duration, hours: {settings.PlayingTimeDurationHours}");

            _watchThread = new Thread(DoThreadJob);
            _watchThread.Start();
        }

        private void DoThreadJob()
        {
            while (true)
            {
                try
                {
                    DoJobQuant();
                }
                catch (Exception e)
                {
                    _log.Error($"{GetType().Name}: Failed with error.");
                    _log.Error(e);
                }
                Thread.Sleep(_refreshProcessesTimeout);
            }
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

        protected abstract void DoJobQuant();
    }
}