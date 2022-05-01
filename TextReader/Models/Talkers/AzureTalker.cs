namespace TextReader.Models.Talkers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using Microsoft.CognitiveServices.Speech;
    using Microsoft.CognitiveServices.Speech.Audio;
    using NAudio.Wave;
    using TextReader.Models.DBs;

    public class AzureTalker : ITalker
    {
        private readonly DispatcherTimer waitTimer = new DispatcherTimer();
        private DirectoryInfo outputDirectoryInfo = new DirectoryInfo("Output");
        private WaveOut waveOut;
        private TimeSpan blankLineWaitTime = new TimeSpan(0, 0, 3);

        public AzureTalker()
        {
            if (!outputDirectoryInfo.Exists)
            {
                outputDirectoryInfo.Create();
            }

            waitTimer.Interval = BlankLineWaitTime;

            waitTimer.Tick += (sender, e) =>
            {
                waitTimer.Stop();
                TalkStopped?.Invoke(this, EventArgs.Empty);
            };
        }

        public event EventHandler TalkStopped;

        public string OutputFileName { get; private set; }

        public bool CanPlay => true;

        public int TalkSpeed { get; set; }

        public int MaxTalkSpeed => 0;

        public int MinTalkSpeed => 0;

        public int Volume { get; set; } = 100;

        public string TalkerName => "Azure Text to Speech";

        public TalkerSetting Setting
        {
            get
            {
                return new TalkerSetting()
                {
                    TalkerID = this.TalkerID,
                    TalkSpeed = this.TalkSpeed,
                    Volume = this.Volume,
                };
            }

            set
            {
                if (TalkerID != value.TalkerID)
                {
                    return;
                }

                TalkSpeed = value.TalkSpeed;
                Volume = value.Volume;
            }
        }

        public string SecretKeyVariableName { get; set; } = "Microsoft_Speech_Secret_key";

        public TimeSpan BlankLineWaitTime
        {
            get => blankLineWaitTime;
            set
            {
                blankLineWaitTime = value;
                waitTimer.Interval = blankLineWaitTime;
            }
        }

        public int TalkerID => 2;

        public void Stop()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
            }
        }

        public async void Talk(TextRecord textRecord)
        {
            if (string.IsNullOrEmpty(textRecord.Text))
            {
                waitTimer.Start();
                return;
            }

            if (string.IsNullOrWhiteSpace(textRecord.OutputFileName))
            {
                await StartTalk(textRecord);
            }
            else
            {
                waveOut = new WaveOut();
                waveOut.Init(new AudioFileReader($"{outputDirectoryInfo.Name}\\{textRecord.OutputFileName}"));
                waveOut.Play();

                waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
            }
        }

        private async Task StartTalk(TextRecord record)
        {
            string key = Environment.GetEnvironmentVariable(SecretKeyVariableName);

            var config = SpeechConfig.FromSubscription(key, "japaneast");
            config.SpeechSynthesisLanguage = "ja-JP";
            config.SpeechSynthesisVoiceName = "ja-JP-KeitaNeural";

            OutputFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssff")}.wav";
            record.OutputFileName = OutputFileName;
            var audioConfig = AudioConfig.FromWavFileOutput($"{outputDirectoryInfo.Name}\\{OutputFileName}");

            using (var synthesizer = new SpeechSynthesizer(config, audioConfig))
            {
                await synthesizer.SpeakTextAsync(record.Text);
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
