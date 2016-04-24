using System;
using NAudio.Wave;

namespace GameBuster.Services
{
    class SoundService
    {
        public void Play(string fileName)
        {
            WaveStream mainOutputStream = CreateWaveStream(fileName);
            WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);

            WaveOutEvent player = new WaveOutEvent();

            player.Init(volumeStream);

            player.Play();
        }

        private static WaveStream CreateWaveStream(string file)
        {
            if (file.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                return new WaveFileReader(file);

            if (file.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                return new Mp3FileReader(file);

            return new MediaFoundationReader(file);
        }

        public string[] GetSupportedAudioFormats()
        {
            return new[] {"wav", "mp3"};
        }
    }
}
