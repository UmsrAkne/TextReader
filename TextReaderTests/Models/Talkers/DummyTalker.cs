namespace TextReaderTests.Models.Talkers
{
    using System;
    using TextReader.Models.Talkers;

    public class DummyTalker : ITalker
    {
        public event EventHandler TalkStopped;

        public string LastReadMessage { get; private set; } = string.Empty;

        public bool Talking { get; private set; }

        public bool CanPlay { get; set; }

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
