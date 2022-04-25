namespace TextReaderTests.Models.Talkers
{
    using System;
    using TextReader.Models.DBs;
    using TextReader.Models.Talkers;

    public class DummyTalker : ITalker
    {
        public event EventHandler TalkStopped;

        public string LastReadMessage { get; private set; } = string.Empty;

        public bool Talking { get; private set; }

        public bool CanPlay { get; set; } = true;

        public int TalkSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int MaxTalkSpeed => throw new NotImplementedException();

        public int MinTalkSpeed => throw new NotImplementedException();

        public string TalkerName => "dummyTalker";

        public void Stop()
        {
            Talking = false;
        }

        public void Talk(TextRecord textRecord)
        {
            LastReadMessage = textRecord.Text;
            Talking = true;
        }

        public void FinishTalk()
        {
            Talking = false;
            TalkStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}
