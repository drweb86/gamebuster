namespace GameBuster.Model
{
    public class GameBusterSettings
    {
        /// <summary>
        /// User can define his own music file.
        /// </summary>
        public string AlarmSoundFile { get; set; }

        public GameBusterSettings() // serialization.
        {

        }

        public GameBusterSettings(string alarmSoundFile) // serialization.
        {
            AlarmSoundFile = alarmSoundFile;
        }
    }
}
