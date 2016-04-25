using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GameBuster.Controller;
using HDE.Platform.Logging;
using Microsoft.Win32;

namespace GameBuster.Services
{
    class StartupService
    {
        private readonly ILog _log;

        public StartupService(ILog log)
        {
            _log = log;
        }

        public void StartOnUserLogin()
        {
            _log.Info("Checking if application will start with user login.");

            const string applicationKey = "GameBuster";
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if(runKey.GetValue(applicationKey) != FileHelper.Executable)
            {
                _log.Info("Added application startup with login of current user.");
                runKey.SetValue(applicationKey, FileHelper.Executable);
            }
            else
            {
                _log.Debug("Application is starting with Windows start.");
            }
        }
    }
}
