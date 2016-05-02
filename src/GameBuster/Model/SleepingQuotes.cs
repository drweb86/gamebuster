using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBuster.Model
{
    class SleepingQuotes
    {
        private static readonly string[] _messages = 
            {
                "My Overlord, you need to go to sleep. New day will bring new battles and victories.",
                "Overlord, your bed is waiting for you to perform the sleeping quest.",
                "Sleeping quest is waiting you, Overlord.",
                "Time to become the king of the night, Overlord.",
                "Its time to perform a journey to the sleep, Overlord.",
                "The new day will bring you the new force, Overlord.",
                "Go sleeping and let the Force to be with you, Overlord.",
            };

        public static string GetRandomGoSleepingMessage()
        {
            Random random = new Random();
            return _messages[random.Next(_messages.Length)];
        }
    }
}
