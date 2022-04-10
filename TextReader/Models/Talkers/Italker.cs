namespace TextReader.Models.Talkers
{
    using System;

    public interface Italker
    {
        event EventHandler TalkStopped;

        void Talk(string message);

        void Stop();
    }
}
