﻿namespace TextReader.Models.Talkers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Threading;

    public class BouyomiTalker : ITalker
    {
        private DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
        private DispatcherTimer waitTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(3000) };

        public BouyomiTalker()
        {
            timer.Tick += Timer_Tick;
            waitTimer.Tick += Wait; 
        }

        public event EventHandler TalkStopped;

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Talk(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                waitTimer.Start();
            }
            else
            {
                ExecuteRemoteTalk($"/Talk {str}");
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
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
