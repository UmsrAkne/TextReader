namespace TextReader.Models.Talkers
{
    using System;

    public interface ITalker
    {
        event EventHandler TalkStopped;

        bool CanPlay { get; }

        void Talk(string message);

        void Stop();
    }
}
