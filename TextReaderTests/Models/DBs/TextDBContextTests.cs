namespace TextReader.Models.DBs.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TextDBContextTests
    {
        [TestMethod]
        public void GetTextsTest()
        {
            var option = new DbContextOptionsBuilder<TextDBContext>()
            .UseInMemoryDatabase(databaseName: "TestMemoryDB")
            .Options;

            var db = new TextDBContext(option);
            db.Database.EnsureCreated();

            db.AddTexts(new List<TextRecord>()
            {
                new TextRecord()
                {
                    Title = "title1",
                    TitleNumber = 0,
                    Text = "text1",
                    CreationDateTime = new DateTime(2000, 1, 1)
                },
                new TextRecord()
                {
                    Title = "title1",
                    TitleNumber = 0,
                    Text = "text2",
                    CreationDateTime = new DateTime(2000, 1, 1)
                },
                new TextRecord()
                {
                    Title = "title2",
                    TitleNumber = 1,
                    Text = "text3",
                    CreationDateTime = new DateTime(2000, 1, 1)
                }
            });

            var result = db.GetTexts(titleNumber: 0);
            Assert.AreEqual(result.Count(), 2);
            Assert.AreEqual(result[0].Text, "text1");
            Assert.AreEqual(result[1].Text, "text2");
        }
    }
}