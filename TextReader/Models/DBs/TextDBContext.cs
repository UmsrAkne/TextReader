﻿namespace TextReader.Models.DBs
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

        private DBQueryer DBQueryer { get; set; }

        private DbSet<TextRecord> Texts { get; set; }

        private DbSet<TitleRecord> Titles { get; set; }

        public List<TextRecord> GetTexts(string title, DateTime dateTime)
        {
            return DBQueryer.GetTexts(title, dateTime).ToList();
        }

        public void AddTexts(List<TextRecord> texts)
        {
            DBQueryer.AddTexts(texts);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseFileName = "TextDB.sqlite";

            if (!File.Exists(databaseFileName))
            {
                SQLiteConnection.CreateFile(databaseFileName); // ファイルが存在している場合は問答無用で上書き。
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = databaseFileName }.ToString();
            optionsBuilder.UseSqlite(new SQLiteConnection(connectionString));
        }
    }
}
