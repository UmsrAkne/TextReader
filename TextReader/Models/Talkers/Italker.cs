namespace TextReader.Models.Talkers
{
    using System;

    public interface ITalker
    {
        event EventHandler TalkStopped;

        void Talk(string message);

        void Stop();
    }
}
