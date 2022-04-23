namespace TextReader.Models.Talkers
{
    using System;
    using TextReader.Models.DBs;

    public interface ITalker
    {
        event EventHandler TalkStopped;

        bool CanPlay { get; }

        int TalkSpeed { get; set; }

        int MaxTalkSpeed { get; }

        int MinTalkSpeed { get; }

        int Volume { get; set; }

        void Talk(TextRecord textRecord);

        void Stop();
    }
}
