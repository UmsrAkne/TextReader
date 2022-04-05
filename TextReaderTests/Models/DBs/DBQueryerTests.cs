namespace TextReader.Models.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TextReader.Models.DBs;

    [TestClass]
    public class DBQueryerTests
    {
        [TestMethod]
        public void GetTextsFromTitleIDTest()
        {
            DBQueryer queryer = new DBQueryer();
            List<TextRecord> records = new List<TextRecord>();

            records.Add(new TextRecord() { Text = "text1", TitleNumber = 1, Index = 1 });
            records.Add(new TextRecord() { Text = "text2", TitleNumber = 1, Index = 1 });
            records.Add(new TextRecord() { Text = "text3", TitleNumber = 2, Index = 1 });

            queryer.Target = records;
            var results = queryer.GetTexts(1);

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results.ToList()[0].Text, "text1");
            Assert.AreEqual(results.ToList()[1].Text, "text2");

            Assert.AreEqual(queryer.GetTexts(2).Count(), 1);
            Assert.AreEqual(queryer.GetTexts(2).ToList().ToList()[0].Text, "text3");

            Assert.AreEqual(queryer.GetTexts(3).Count(), 0);
        }

        [TestMethod]
        public void GetTextsTest()
        {
            DBQueryer queryer = new DBQueryer();
            List<TextRecord> records = new List<TextRecord>();

            records.Add(new TextRecord()
            {
                Title = "testTitle",
                CreationDateTime = new DateTime(2022, 12, 10),
                Text = "text1",
                Index = 1
            });

            records.Add(new TextRecord()
            {
                Title = "testTitle",
                CreationDateTime = new DateTime(2022, 12, 10),
                Text = "text2",
                Index = 2
            });

            records.Add(new TextRecord()
            {
                Title = "testTitle",
                CreationDateTime = new DateTime(2022, 12, 10),
                Text = "text3",
                Index = 3
            });

            records.Add(new TextRecord()
            {
                Title = "testTitle",
                CreationDateTime = new DateTime(2022, 12, 11),
                Text = "text1",
                Index = 1
            });

            queryer.Target = records;

            var texts = queryer.GetTexts("testTitle", new DateTime(2022, 12, 10)).ToList();

            Assert.AreEqual(texts.Count(), 3);
            Assert.AreEqual(texts[0].Text, "text1");
            Assert.AreEqual(texts[1].Text, "text2");
            Assert.AreEqual(texts[2].Text, "text3");
        }
    }
}