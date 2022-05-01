namespace TextReader.Models.Talkers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;
    using TextReader.Models.DBs;

    public class BouyomiTalker : ITalker
    {
        private DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
        private DispatcherTimer waitTimer = new DispatcherTimer();
        private int playingCheckWaitCounter;
        private TimeSpan blankLineWaitTime = new TimeSpan(0, 0, 3);

        public BouyomiTalker()
        {
            timer.Tick += Timer_Tick;
            waitTimer.Interval = BlankLineWaitTime;
            waitTimer.Tick += Wait;
            TalkSpeed = DefaultTalkSpeed;
        }

        public event EventHandler TalkStopped;

        public bool CanPlay => Process.GetProcessesByName("BouyomiChan").Length > 0;

        public int TalkSpeed { get; set; }

        public int Volume { get; set; } = 100;

        public int MaxTalkSpeed => 300;

        public int MinTalkSpeed => 50;

        public int DefaultTalkSpeed => 100;

        public string TalkerName => "棒読みちゃん";

        public TimeSpan BlankLineWaitTime
        {
            get => blankLineWaitTime;
            set
            {
                blankLineWaitTime = value;
                waitTimer.Interval = blankLineWaitTime;
            }
        }

        public int TalkerID => 1;

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

        public void Stop()
        {
            ExecuteRemoteTalk($"/Clear");
        }

        public void Talk(TextRecord record)
        {
            /// 他の実装クラスの Talk() との兼ね合いで TextRecord を引数に取っているが、
            /// このクラスで必要なのはテキストのみなので、テキストだけ取り出して利用する。
            var message = record.Text;

            if (string.IsNullOrWhiteSpace(message))
            {
                waitTimer.Start();
            }
            else
            {
                /// Talk の引数は 入力文章 速度 音程 音量 話者ID の順となっている。
                ExecuteRemoteTalk($"/Talk {message} {TalkSpeed} -1 {Volume} 0");
                playingCheckWaitCounter = (int)Math.Ceiling((decimal)message.Length / 30);
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            playingCheckWaitCounter--;

            if (playingCheckWaitCounter > 0)
            {
                return;
            }

            var process = ExecuteRemoteTalk($"/GetNowPlaying");
            process.WaitForExit();

            /// GetNowPlaying を実行した時、音声を再生中ならば 1 、再生中でなければ 0 が終了コードで返ってくる

            if (process.ExitCode == 0)
            {
                timer.Stop();
                TalkStopped?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Wait(object sender, EventArgs e)
        {
            waitTimer.Stop();
            TalkStopped?.Invoke(this, EventArgs.Empty);
        }

        private Process ExecuteRemoteTalk(string argument)
        {
            var process = new Process();
            process.StartInfo.FileName = @"BouyomiChan\RemoteTalk\RemoteTalk.exe";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.Arguments = argument;
            process.Start();

            return process;
        }
    }
}
