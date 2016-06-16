using System;
using System.Collections.Generic;
using System.Diagnostics;
using HDE.Platform.Logging;

namespace GameBuster.Services
{
    class ProcessHelperService
    {
        private readonly ILog _log;
        private readonly int _sessionId;
        private readonly List<string> _knownNonGames = new List<string>
        {
            "ApplicationFrameHost", "Calculator", "ConEmu64", "devenv", "ScriptedSandbox64", "ShellExperienceHost",
            "PROCEXP64", "Far", "firefox", "KeePass", "mmc", "SnagitEditor", "explorer"
        };

        public ProcessHelperService(ILog log)
        {
            _log = log;
            _sessionId = Process.GetCurrentProcess().SessionId;
        }

        public IEnumerable<string> GetCurrentUserProcessNames(bool withWindow, bool excludeKnownNonGames)
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
                        _log.Info($"- {process.ProcessName}");

                        if (withWindow && process.MainWindowHandle == IntPtr.Zero)
                            continue;

                        if (excludeKnownNonGames && _knownNonGames.Contains(process.ProcessName))
                            continue;

                        currentUserProcessNames.Add(process.ProcessName);
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
    }
}
