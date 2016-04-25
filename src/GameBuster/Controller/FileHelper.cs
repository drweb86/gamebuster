using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameBuster.Controller
{
    class FileHelper
    {
        public static string LogsFolder { get; private set; }
        public static string SettingsFolder { get; private set; }
        public static string Executable { get; set; }

        static FileHelper ()
        {
            LogsFolder = Path.Combine(Path.GetTempPath(), "GameBuster");
            if (!Directory.Exists(LogsFolder))
                Directory.CreateDirectory(LogsFolder);

            SettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GameBuster");
            if (!Directory.Exists(SettingsFolder))
                Directory.CreateDirectory(SettingsFolder);

            Executable = Assembly.GetExecutingAssembly().Location;
        }
    }
}
