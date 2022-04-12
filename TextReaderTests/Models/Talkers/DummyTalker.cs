namespace TextReaderTests.Models.Talkers
{
    using System;
    using TextReader.Models.Talkers;

    public class DummyTalker : ITalker
    {
        public event EventHandler TalkStopped;

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Talk(string message)
        {
            throw new NotImplementedException();
        }
    }
}
