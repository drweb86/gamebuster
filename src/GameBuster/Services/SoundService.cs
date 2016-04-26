using System;
using System.IO;
using System.Linq;
using HDE.Platform.Logging;
using NAudio.Wave;

namespace GameBuster.Services
{
    class SoundService
    {
        private readonly ILog _log;

        public SoundService(ILog log)
        {
            _log = log;
        }

        public void Play(string fileName)
        {
            _log.Debug($"Playing file: {fileName}");
            WaveStream mainOutputStream = CreateWaveStream(fileName);
            WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);

            WaveOutEvent player = new WaveOutEvent();

            player.Init(volumeStream);

            player.Play();
        }

        public string FindAlarmSound(string preferredFile)
        {
            _log.Info($"Searching for alarm sound. Preferred file is {preferredFile}...");

            return (!string.IsNullOrWhiteSpace(preferredFile) && File.Exists(preferredFile)) ?
                preferredFile :
                FindSystemRandomSound();
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

        private string FindSystemRandomSound()
        {
            var soundsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Media");
            if (!Directory.Exists(soundsFolder))
                throw new ApplicationException($"Can't locate directory {soundsFolder}");

            var allWavFiles = Directory.GetFiles(soundsFolder, "Alarm*.wav").ToList();
            allWavFiles.AddRange(Directory.GetFiles(soundsFolder, "Ring*.wav"));
            allWavFiles.AddRange(Directory.GetFiles(soundsFolder, "tada.wav"));

            if (allWavFiles.Count == 0)
                throw new ApplicationException($"Can't locate Alarm*.wav/Ring*.wav/tada.wav music files in {soundsFolder}.");

            var random = new Random();
            return allWavFiles[random.Next(allWavFiles.Count)];
        }
    }
}
