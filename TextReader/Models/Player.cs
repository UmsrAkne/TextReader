/// <summary>
/// インスタンス内に IPlayer を実装したクラスを内蔵。
/// それを使って入力されたテキスト（リスト）の読み上げを行うクラスです。
/// </summary>
namespace TextReader.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TextReader.Models.DBs;
    using TextReader.Models.Talkers;

    public class Player
    {
        private ITalker talker;
        private List<TextRecord> texts;
        private int index;
        private TextRecord currentRecord;

        public event EventHandler PlayStarted;

        public ITalker Talker
        {
            get => talker;
            set => talker = value;
        }

        public List<TextRecord> Texts
        {
            get => texts;
            set => texts = value;
        }

        public int Index
        {
            get => index;
            set => index = value;
        }

        public void Play()
        {
            if (currentRecord != null)
            {
                currentRecord.IsPlaying = false;
            }

            if (Texts.Count() > Index && Talker.CanPlay)
            {
                currentRecord = Texts[Index];
                talker.Talk(currentRecord.Text);
                currentRecord.ListenCount++;
                currentRecord.IsPlaying = true;

                PlayStarted?.Invoke(this, EventArgs.Empty);
                Index++;
                talker.TalkStopped += PlayNext;
            }
        }

        public void Stop()
        {
            talker.Stop();
            talker.TalkStopped -= PlayNext;
            Index = 0;
        }

        public void Pause()
        {
            talker.Stop();
            talker.TalkStopped -= PlayNext;
        }

        private void PlayNext(object sender, EventArgs e)
        {
            talker.TalkStopped -= PlayNext;
            Play();
        }
    }
}
