namespace TextReader.Models.Talkers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Threading;

    public class BouyomiTalker : Italker
    {
        public event EventHandler TalkStopped;

        private DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };

        public BouyomiTalker()
        {
            timer.Tick += Timer_Tick;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Talk(string str)
        {
            var process = new Process();
            process.StartInfo.FileName = @"BouyomiChan\RemoteTalk\RemoteTalk.exe";
            process.StartInfo.RedirectStandardOutput = true;

            // 標準出力を使うためには false にセットする必要があるみたい
            process.StartInfo.UseShellExecute = false;

            string commandText = $"/Talk {str}";
            process.StartInfo.Arguments = $"{commandText}";
            process.Start();

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var process = new Process();
            process.StartInfo.FileName = @"BouyomiChan\RemoteTalk\RemoteTalk.exe";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.Arguments = $"/GetNowPlaying";
            process.Start();
            process.WaitForExit();

            // GetNowPlaying を実行した時、音声を再生中ならば 1 、再生中でなければ 0 が終了コードで返ってくる

            if(process.ExitCode == 0)
            {
                timer.Stop();
                TalkStopped?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
