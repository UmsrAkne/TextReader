namespace TextReader.Models.Talkers
{
    using System.Diagnostics;

    public class BouyomiTalker
    {
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
        }
    }
}
