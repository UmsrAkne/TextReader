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
        }

        public TextDBContext(DbContextOptions<TextDBContext> options) : base(options)
        {
        }

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

        public List<TitleRecord> GetTitles()
        {
            return Titles.OrderBy(record => record.CreationDateTime).ToList();
        }

        public int AddTitle(string title)
        {
            var titleRecord = new TitleRecord()
            {
                Title = title,
                CreationDateTime = DateTime.Now
            };

            Titles.Add(titleRecord);

            SaveChanges();

            // Id に関しては EntityFW が自動でインクリメントで値を振る。
            // titleRecord の中の値はそれに連動して更新されるので、直前に入力したレコードの id が取得できる。
            return titleRecord.Id;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
