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
        private Italker talker;
        private List<TextRecord> texts;
        private int index;

        public Italker Talker
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
            if (Texts.Count() > Index)
            {
                talker.Talk(Texts[Index].Text);
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
