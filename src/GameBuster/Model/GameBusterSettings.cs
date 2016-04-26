using System;
using System.Collections.Generic;

namespace GameBuster.Model
{
    public class GameBusterSettings
    {
        private readonly bool _isDeserialization;
        public const int MinimumPlayingTimeDurationHours = 2;

        /// <summary>
        /// User can define his own music file.
        /// </summary>
        public string AlarmSoundFile { get; set; }

        private int _playingTimeDurationHours;

        public int PlayingTimeDurationHours
        {
            get { return _playingTimeDurationHours; }
            set
            {
                if (value < _playingTimeDurationHours)
                    if (_isDeserialization) // support for old options
                    {
                        value = _playingTimeDurationHours;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"Playing time duration is out of range. It must be more or equal to {_playingTimeDurationHours}");
                    }

                _playingTimeDurationHours = value;
            }
        }

        private List<string> _knownGames;

        public List<string> KnownGames
        {
            get { return _knownGames; }
            set
            {
                if (value == null)
                    if (_isDeserialization) // support for old options
                    {
                        value = new List<string>();
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(value), $"Known games list is empty.");
                    }

                _knownGames = value;
            }
        }

        public GameBusterSettings() // serialization.
        {
            _isDeserialization = true;
            PlayingTimeDurationHours = MinimumPlayingTimeDurationHours;
            KnownGames = new List<string>();
        }

        public GameBusterSettings(string alarmSoundFile, int playingTimeDurationHours, List<string> knownGames) // serialization.
        {
            AlarmSoundFile = alarmSoundFile;
            PlayingTimeDurationHours = playingTimeDurationHours;
            KnownGames = knownGames;
        }
    }
}
