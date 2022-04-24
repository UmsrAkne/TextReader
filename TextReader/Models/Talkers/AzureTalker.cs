namespace TextReader.Models.Talkers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.CognitiveServices.Speech;
    using Microsoft.CognitiveServices.Speech.Audio;
    using TextReader.Models.DBs;
    using TextReader.Models.Talkers;
    // using WMPLib;

    public class AzureTalker : ITalker
    {
        private DirectoryInfo outputDirectoryInfo = new DirectoryInfo("Output");

        public AzureTalker()
        {
            if (!outputDirectoryInfo.Exists)
            {
                outputDirectoryInfo.Create();
            }

            //WMP.PlayStateChange += (int NewState) =>
            //{
            //    if (WMP.playState == WMPPlayState.wmppsMediaEnded)
            //    {
            //        TalkEnded?.Invoke(this, new EventArgs());
            //    }
            //};
        }

        public event EventHandler TalkEnded;

        public event EventHandler TalkStopped;

        public string OutputFileName { get; private set; }

        public bool CanPlay => true;

        public int TalkSpeed { get; set; }

        public int MaxTalkSpeed => 100;

        public int MinTalkSpeed => 0;

        public int Volume { get; set; }

        // private WindowsMediaPlayer WMP { get; } = new WindowsMediaPlayer();

        public void Stop()
        {
            // WMP.controls.stop();
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
                // await synthesizer.SpeakSsmlAsync(ssml);
                await synthesizer.SpeakTextAsync(ssml);
            }

            // WMP.URL = $"{outputDirectoryInfo.Name}\\{OutputFileName}";
            // WMP.controls.play();
        }

        public async void Talk(TextRecord textRecord)
        {
            await Talk(textRecord.Text);
        }
    }
}
