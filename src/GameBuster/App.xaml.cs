using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GameBuster.Controller;
using Hardcodet.Wpf.TaskbarNotification;

namespace GameBuster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;
        private readonly GameBusterController _controller = GameBusterController.Controller;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            AppDomain.CurrentDomain.UnhandledException += (sender, exc)
                => FatalExceptionObject(exc.ExceptionObject);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner

            GameBusterController.Controller.Dispose();

            base.OnExit(e);
        }

        private void FatalExceptionObject(object exceptionObject)
        {
            GameBusterController.Controller.Log.Error((Exception)exceptionObject);
        }
    }
}
