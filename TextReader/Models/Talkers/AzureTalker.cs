namespace TextReader.Models.Talkers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using TextReader.Models.DBs;
    using TextReader.Models.Talkers;
    using Microsoft.CognitiveServices.Speech;
    using Microsoft.CognitiveServices.Speech.Audio;
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

        public bool CanPlay => throw new NotImplementedException();

        public int TalkSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int MaxTalkSpeed => throw new NotImplementedException();

        public int MinTalkSpeed => throw new NotImplementedException();

        public int Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // private WindowsMediaPlayer WMP { get; } = new WindowsMediaPlayer();

        public async void SSMLTalk(string ssmlText)
        {
            await Talk(ssmlText);
        }

        public void Stop()
        {
            // WMP.controls.stop();
        }

        private async Task Talk(string ssml)
        {
            // string key = Environment.GetEnvironmentVariable("Microsoft_Speech_Secret_key");

            // var config = SpeechConfig.FromSubscription(key, "japaneast");
            // config.SpeechSynthesisLanguage = "ja-JP";
            // config.SpeechSynthesisVoiceName = "ja-JP-KeitaNeural";

            // OutputFileName = DateTime.Now.ToString("yyyyMMddHHmmssff") + ".wav";
            // var audioConfig = AudioConfig.FromWavFileOutput($"{outputDirectoryInfo.Name}\\{OutputFileName}");

            // using (var synthesizer = new SpeechSynthesizer(config, audioConfig))
            // {
            //     var ssmlGen = new AzureSSMLGen();
            //     await synthesizer.SpeakSsmlAsync(ssml);
            // }

            // WMP.URL = $"{outputDirectoryInfo.Name}\\{OutputFileName}";
            // WMP.controls.play();
        }

        public void Talk(TextRecord textRecord)
        {
            throw new NotImplementedException();
        }
    }
}
