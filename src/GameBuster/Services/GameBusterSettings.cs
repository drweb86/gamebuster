using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBuster.Services
{
    public class GameBusterSettings
    {
        /// <summary>
        /// User can define his own music file.
        /// </summary>
        public string AlertFile { get; set; }

        public GameBusterSettings() // serialization.
        {

        }
    }
}
