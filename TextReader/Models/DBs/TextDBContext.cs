namespace TextReader.Models.DBs
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    public class TextDBContext : DbContext
    {
        public TextDBContext()
        {
            DBQueryer = new DBQueryer();
            DBQueryer.Target = Texts;
            DBQueryer.Titles = Titles;
        }

        public TextDBContext(DbContextOptions<TextDBContext> options) : base(options)
        {
        }

        private DBQueryer DBQueryer { get; set; }

        private DbSet<TextRecord> Texts { get; set; }

        private DbSet<TitleRecord> Titles { get; set; }

        public static DbContextOptions<TextDBContext> CreateDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TextDBContext>();
            string databaseFileName = "TextDB.sqlite";

            if (!File.Exists(databaseFileName))
            {
                SQLiteConnection.CreateFile(databaseFileName); // ファイルが存在している場合は問答無用で上書き。
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = databaseFileName }.ToString();
            return optionsBuilder.UseSqlite(new SQLiteConnection(connectionString)).Options;
        }

        public List<TextRecord> GetTexts(string title, DateTime dateTime)
        {
            return DBQueryer.GetTexts(title, dateTime).ToList();
        }

        public List<TextRecord> GetTexts(int titleNumber)
        {
            return Texts
            .Where(record => titleNumber == record.TitleNumber)
            .OrderBy(record => record.Index)
            .ToList();
        }

        public void AddTexts(List<TextRecord> texts)
        {
            Texts.AddRange(texts);
            SaveChanges();
        }

        public IEnumerable<TitleRecord> GetTitles()
        {
            return Titles.OrderBy(record => record.CreationDateTime);
        }

        public void AddTitle(string title)
        {
            Titles.ToList().Add(new TitleRecord()
            {
                Title = title,
                CreationDateTime = DateTime.Now
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
