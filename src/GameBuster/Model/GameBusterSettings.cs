using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBuster.Model
{
    public class GameBusterSettings
    {
        private readonly bool _isDeserialization;

        public const int MinimumPlayingTimeDurationHours = 2;

        public const int MinKillGameIntervalHour = 0;
        public const int MaxKillGameIntervalHour = 23;

        public const int DefaultKillGameIntervalBeginHour = 2;
        public const int DefaultKillGameIntervalEndHour = 7;

        private int _beginKillGameIntervalHour;

        public static bool IsIntervalHourValid(int hour)
        {
            return hour <= MaxKillGameIntervalHour &&
                   hour >= MinKillGameIntervalHour;
        }

        public int BeginKillGameIntervalHour
        {
            get { return _beginKillGameIntervalHour; }
            set
            {
                if (!IsIntervalHourValid(value))
                    throw new ArgumentOutOfRangeException(nameof(BeginKillGameIntervalHour));

                _beginKillGameIntervalHour = value;
            }
        }


        private int _endKillGameIntervalHour;

        public int EndKillGameIntervalHour
        {
            get { return _endKillGameIntervalHour; }
            set
            {
                if (!IsIntervalHourValid(value))
                    throw new ArgumentOutOfRangeException(nameof(EndKillGameIntervalHour));

                _endKillGameIntervalHour = value;
            }
        }


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

        public bool IsKnown(string game)
        {
            return _knownGames.Any(item => string.Compare(item, game, StringComparison.OrdinalIgnoreCase) == 0);
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

        public GameBusterSettings(
            string alarmSoundFile, 
            int playingTimeDurationHours, 
            List<string> knownGames,
            int beginKillGameIntervalHour,
            int endKillGameIntervalHour)
        {
            AlarmSoundFile = alarmSoundFile;
            PlayingTimeDurationHours = playingTimeDurationHours;
            KnownGames = knownGames;
            BeginKillGameIntervalHour = beginKillGameIntervalHour;
            EndKillGameIntervalHour = endKillGameIntervalHour;
        }
    }
}
