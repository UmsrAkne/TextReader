using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;
using TextReader.Models.DBs;

namespace TextReader.Models.Talkers
{
    public class AzureTalker : ITalker
    {
        private readonly DispatcherTimer waitTimer = new DispatcherTimer();
        private readonly DirectoryInfo outputDirectoryInfo = new DirectoryInfo("Output");
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
            get =>
                new TalkerSetting()
                {
                    TalkerID = TalkerID,
                    TalkSpeed = TalkSpeed,
                    BlankLineWaitTime = (int)BlankLineWaitTime.TotalMilliseconds,
                    AzureTTSKeyVariableName = SecretKeyVariableName,
                    Volume = Volume,
                };

            set
            {
                if (TalkerID == value.TalkerID)
                {
                    TalkSpeed = value.TalkSpeed;
                    Volume = value.Volume;
                    BlankLineWaitTime = TimeSpan.FromMilliseconds(value.BlankLineWaitTime);
                    SecretKeyVariableName = value.AzureTTSKeyVariableName;
                }
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

        public string BlankSoundFilePath { get; set; } = "blankSound.wav";

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
            if (string.IsNullOrWhiteSpace(textRecord.OutputFileName))
            {
                if (string.IsNullOrWhiteSpace(textRecord.Text))
                {
                    await CreateBlankAudioFile(textRecord);
                    return;
                }

                await StartTalk(textRecord);
            }
            else
            {
                PlayWaveOut($"{GetOutputDirectoryPath(textRecord)}\\{textRecord.OutputFileName}");
            }
        }

        private Task CreateBlankAudioFile(TextRecord record)
        {
            OutputFileName = $"{DateTime.Now:yyyyMMddHHmmssff}_blank.wav";
            var outputDirectoryPath = GetOutputDirectoryPath(record);
            File.Copy(BlankSoundFilePath, $"{outputDirectoryPath}\\{OutputFileName}");
            PlayWaveOut($"{outputDirectoryPath}\\{OutputFileName}");
            record.OutputFileName = OutputFileName;
            return Task.CompletedTask;
        }

        private async Task StartTalk(TextRecord record)
        {
            string key = Environment.GetEnvironmentVariable(SecretKeyVariableName);

            // ReSharper disable once StringLiteralTypo / 引数の region は変更不可のため　
            var config = SpeechConfig.FromSubscription(key, "japaneast");

            config.SpeechSynthesisLanguage = "ja-JP";
            config.SpeechSynthesisVoiceName = "ja-JP-KeitaNeural";

            var outputDirectoryPath = GetOutputDirectoryPath(record);
            if (!Directory.Exists(outputDirectoryPath))
            {
                Directory.CreateDirectory(outputDirectoryPath);
            }

            OutputFileName = $"{DateTime.Now:yyyyMMddHHmmssff}.wav";
            record.OutputFileName = OutputFileName;
            var audioConfig = AudioConfig.FromWavFileOutput($"{outputDirectoryPath}\\{OutputFileName}");

            using (var synthesizer = new SpeechSynthesizer(config, audioConfig))
            {
                await synthesizer.SpeakTextAsync(record.Text);
            }

            PlayWaveOut($"{outputDirectoryPath}\\{OutputFileName}");
        }

        private void PlayWaveOut(string audioFilePath)
        {
            var wavFile = new FileInfo(audioFilePath);

            if (wavFile.Exists && wavFile.Length == 0)
            {
                // 読み上げるテキストに不正な文字列が含まれている場合、
                // サイズ 0 の wav ファイルが生成される。
                // これを WaveOut() を使って再生すると例外がスローされる。
                // そのため、サイズが 0 だった場合は waitTimer を起動。

                // 尚、このブロックが実行される時点では、まだハンドラがセットされていないので、
                // 即終了イベントを飛ばしても、次の音声が再生されない。
                waitTimer.Start();
                return;
            }

            var wavFileName = Path.GetFileNameWithoutExtension(wavFile.Name);
            var mp3File = new FileInfo($@"{wavFile.Directory.Name}\{wavFileName}.mp3");

            if (mp3File.Exists)
            {
                waveOut = new WaveOut();
                waveOut.Init(new Mp3FileReader(mp3File.FullName));
            }
            else
            {
                waveOut = new WaveOut();
                waveOut.Init(new AudioFileReader(audioFilePath));
            }

            waveOut.Play();
            waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            waveOut.PlaybackStopped -= WaveOut_PlaybackStopped;
            TalkStopped?.Invoke(this, EventArgs.Empty);
        }

        private string GetOutputDirectoryPath(TextRecord record)
        {
            return $@"{outputDirectoryInfo.Name}\{record.TitleNumber:0000}\";
        }
    }
}