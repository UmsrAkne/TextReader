namespace TextReader.Models.DBs
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    public class TextDbContext : DbContext
    {
        public TextDbContext()
        {
        }

        public TextDbContext(DbContextOptions<TextDbContext> options) : base(options)
        {
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<ListenLog> Histories { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<TalkerSetting> TalkerSettings { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<TextRecord> Texts { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private DbSet<TitleRecord> Titles { get; set; }

        public static DbContextOptions<TextDbContext> CreateDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TextDbContext>();
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

        public TextRecord GetText(int textId) => Texts.First(t => t.Id == textId);

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
                CreationDateTime = DateTime.Now,
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