using System;

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

        public GameBusterSettings() // serialization.
        {
            _isDeserialization = true;
            PlayingTimeDurationHours = MinimumPlayingTimeDurationHours;
        }

        public GameBusterSettings(string alarmSoundFile, int playingTimeDurationHours) // serialization.
        {
            AlarmSoundFile = alarmSoundFile;
            PlayingTimeDurationHours = playingTimeDurationHours;

        }
    }
}
