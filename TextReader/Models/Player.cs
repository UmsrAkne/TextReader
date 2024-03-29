﻿using System;
using System.Collections.Generic;
using System.Linq;
using TextReader.Models.DBs;
using TextReader.Models.Talkers;

namespace TextReader.Models
{
    /// <summary>
    /// インスタンス内に IPlayer を実装したクラスを内蔵。
    /// それを使って入力されたテキスト（リスト）の読み上げを行うクラスです。
    /// </summary>
    ///
    public class Player
    {
        private ITalker talker;
        private TextRecord currentRecord;

        public event EventHandler PlayStarted;

        public event EventHandler PlayStopped;

        public ITalker Talker
        {
            get => talker;
            set => talker = value;
        }

        public List<TextRecord> Texts { get; set; }

        public int Index { get; set; }

        public void Play()
        {
            if (currentRecord != null)
            {
                currentRecord.IsPlaying = false;
            }

            if (Texts.Count() > Index && Talker.CanPlay)
            {
                currentRecord = Texts[Index];
                talker.Talk(currentRecord);
                currentRecord.ListenCount++;
                currentRecord.IsPlaying = true;

                PlayStarted?.Invoke(this, EventArgs.Empty);
                Index++;
                talker.TalkStopped += PlayNext;
            }
            else
            {
                Index = 0;
            }
        }

        public void Stop()
        {
            if (currentRecord != null)
            {
                currentRecord.IsPlaying = false;
            }

            talker.Stop();
            talker.TalkStopped -= PlayNext;
            Index = 0;
            PlayStopped?.Invoke(this, EventArgs.Empty);
        }

        public void Pause()
        {
            talker.Stop();
            talker.TalkStopped -= PlayNext;
        }

        private void PlayNext(object sender, EventArgs e)
        {
            talker.TalkStopped -= PlayNext;
            PlayStopped?.Invoke(this, EventArgs.Empty);
            Play();
        }
    }
}