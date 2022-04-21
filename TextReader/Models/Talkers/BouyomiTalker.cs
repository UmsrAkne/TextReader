namespace TextReader.Models.Talkers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;

    public class BouyomiTalker : ITalker
    {
        private DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
        private DispatcherTimer waitTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(3000) };
        private int playingCheckWaitCounter;

        public BouyomiTalker()
        {
            timer.Tick += Timer_Tick;
            waitTimer.Tick += Wait;
        }

        public event EventHandler TalkStopped;

        public bool CanPlay => Process.GetProcessesByName("BouyomiChan").Length > 0;

        public int TalkSpeed { get; set; }

        public int Volume { get; set; } = 100;

        public void Stop()
        {
            ExecuteRemoteTalk($"/Clear");
        }

        public void Talk(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                waitTimer.Start();
            }
            else
            {
                /// Talk の引数は 入力文章 速度 音程 音量 話者ID の順となっている。
                ExecuteRemoteTalk($"/Talk {str} {TalkSpeed} -1 {Volume} 0");
                playingCheckWaitCounter = (int)Math.Ceiling((decimal)str.Length / 30);
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
