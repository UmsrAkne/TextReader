namespace TextReader.Models.Tests
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TextReader.Models.DBs;
    using TextReaderTests.Models.Talkers;

    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void PlayTest()
        {
            var dummyTalker = new DummyTalker();
            var player = new Player() { Talker = dummyTalker };

            player.Texts = new List<TextRecord>()
            {
                new TextRecord() { Text = "text1" },
                new TextRecord() { Text = "text2" }
            };

            Assert.AreEqual(dummyTalker.LastReadMessage, string.Empty);
            Assert.IsFalse(dummyTalker.Talking);

            Assert.AreEqual(player.Texts[0].ListenCount, 0);
            Assert.AreEqual(player.Texts[1].ListenCount, 0);

            player.Play();

            Assert.AreEqual(dummyTalker.LastReadMessage, "text1");
            Assert.IsTrue(dummyTalker.Talking, "text1 を読み上げ中");
            dummyTalker.FinishTalk();

            Assert.AreEqual(dummyTalker.LastReadMessage, "text2");
            Assert.IsTrue(dummyTalker.Talking, "text2 を読み上げ中");
            dummyTalker.FinishTalk();

            Assert.AreEqual(dummyTalker.LastReadMessage, "text2");
            Assert.IsFalse(dummyTalker.Talking, "読み上げは終了している");

            Assert.AreEqual(player.Texts[0].ListenCount, 1);
            Assert.AreEqual(player.Texts[1].ListenCount, 1);
        }
    }
}