using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBuster.Services;

namespace GameBuster.Model
{
    class GameBusterModel
    {
        public GameBusterSettings Settings { get; set; }

        public GameBusterModel()
        {
            Settings = new GameBusterSettings();
        }
    }
}
