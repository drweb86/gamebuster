namespace GameBuster.Model
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
