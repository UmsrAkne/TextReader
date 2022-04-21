namespace TextReader.Models.Talkers
{
    using System;

    public interface ITalker
    {
        event EventHandler TalkStopped;

        bool CanPlay { get; }

        int TalkSpeed { get; set; }

        int Volume { get; set; }

        void Talk(string message);

        void Stop();
    }
}
