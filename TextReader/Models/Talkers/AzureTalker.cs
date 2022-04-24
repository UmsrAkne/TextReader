namespace TextReader.Models.Talkers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.CognitiveServices.Speech;
    using Microsoft.CognitiveServices.Speech.Audio;
    using NAudio.Wave;
    using TextReader.Models.DBs;

    public class AzureTalker : ITalker
    {
        private DirectoryInfo outputDirectoryInfo = new DirectoryInfo("Output");
        private WaveOut waveOut;

        public AzureTalker()
        {
            if (!outputDirectoryInfo.Exists)
            {
                outputDirectoryInfo.Create();
            }
        }

        public event EventHandler TalkStopped;

        public string OutputFileName { get; private set; }

        public bool CanPlay => true;

        public int TalkSpeed { get; set; }

        public int MaxTalkSpeed => 0;

        public int MinTalkSpeed => 0;

        public int Volume { get; set; } = 100;

        public void Stop()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
            }
        }

        public async void Talk(TextRecord textRecord)
        {
            await Talk(textRecord.Text);
        }

        private async Task Talk(string ssml)
        {
            string key = Environment.GetEnvironmentVariable("Microsoft_Speech_Secret_key");

            var config = SpeechConfig.FromSubscription(key, "japaneast");
            config.SpeechSynthesisLanguage = "ja-JP";
            config.SpeechSynthesisVoiceName = "ja-JP-KeitaNeural";

            OutputFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssff")}.wav";
            var audioConfig = AudioConfig.FromWavFileOutput($"{outputDirectoryInfo.Name}\\{OutputFileName}");

            using (var synthesizer = new SpeechSynthesizer(config, audioConfig))
            {
                await synthesizer.SpeakTextAsync(ssml);
            }

            waveOut = new WaveOut();
            waveOut.Init(new AudioFileReader($"{outputDirectoryInfo.Name}\\{OutputFileName}"));
            waveOut.Play();

            waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            waveOut.PlaybackStopped -= WaveOut_PlaybackStopped;
            TalkStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}
