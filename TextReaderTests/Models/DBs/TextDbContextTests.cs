namespace TextReader.Models.DBs.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TextDbContextTests
    {
        [TestMethod]
        public void GetTextsTest()
        {
            var option = new DbContextOptionsBuilder<TextDbContext>()
            .UseInMemoryDatabase(databaseName: "TestMemoryDB")
            .Options;

            var db = new TextDbContext(option);
            db.Database.EnsureCreated();

            db.AddTexts(new List<TextRecord>()
            {
                new TextRecord()
                {
                    TitleNumber = 0,
                    Text = "text1",
                    CreationDateTime = new DateTime(2000, 1, 1)
                },
                new TextRecord()
                {
                    TitleNumber = 0,
                    Text = "text2",
                    CreationDateTime = new DateTime(2000, 1, 1)
                },
                new TextRecord()
                {
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

        [TestMethod]
        public void AddTitleTest()
        {
            var option = new DbContextOptionsBuilder<TextDbContext>()
            .UseInMemoryDatabase(databaseName: "TestMemoryDB")
            .Options;

            var db = new TextDbContext(option);
            db.Database.EnsureCreated();

            Assert.AreEqual(1, db.AddTitle("title1"));
            Assert.AreEqual(2, db.AddTitle("title2"));
            Assert.AreEqual(3, db.AddTitle("title3"));

            var titles = db.GetTitles();

            Assert.AreEqual(titles.Count(), 3);
            Assert.AreEqual(titles[0].Title, "title1");
            Assert.AreEqual(titles[1].Title, "title2");
            Assert.AreEqual(titles[2].Title, "title3");
        }
    }
}