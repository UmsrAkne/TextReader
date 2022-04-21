namespace TextReaderTests.Models.Talkers
{
    using System;
    using TextReader.Models.Talkers;

    public class DummyTalker : ITalker
    {
        public event EventHandler TalkStopped;

        public string LastReadMessage { get; private set; } = string.Empty;

        public bool Talking { get; private set; }

        public bool CanPlay { get; set; } = true;

        public int TalkSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Stop()
        {
            Talking = false;
        }

        public void Talk(string message)
        {
            LastReadMessage = message;
            Talking = true;
        }

        public void FinishTalk()
        {
            Talking = false;
            TalkStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}
