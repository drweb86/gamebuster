using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GameBuster.Controller;

namespace GameBuster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, exc)
                => FatalExceptionObject(exc.ExceptionObject);

            GameBusterController.Controller.Dispose();

            base.OnExit(e);
        }

        private void FatalExceptionObject(object exceptionObject)
        {
            GameBusterController.Controller.Log.Error((Exception)exceptionObject);
        }
    }
}
