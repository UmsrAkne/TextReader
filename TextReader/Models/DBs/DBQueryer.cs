namespace TextReader.Models.DBs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DBQueryer
    {
        public IEnumerable<TextRecord> Target { get; set; }

        public IEnumerable<TitleRecord> Titles { get; set; }

        public IEnumerable<TextRecord> GetTexts(string title, DateTime dateTime)
        {
            return Target
                .Where(record => title == record.Title && record.CreationDateTime == dateTime)
                .OrderBy(record => record.Index);
        }

        public IEnumerable<TextRecord> GetTexts(int titleNumber)
        {
            return Target
                .Where(record => titleNumber == record.TitleNumber)
                .OrderBy(record => record.Index);
        }

        public IEnumerable<TitleRecord> GetTitles()
        {
            return Titles.OrderBy(record => record.CreationDateTime);
        }

        public void AddTexts(IEnumerable<TextRecord> texts)
        {
            Target.ToList().AddRange(texts);
        }

        public void AddTitle(string title)
        {
            Titles.ToList().Add(new TitleRecord()
            {
                Title = title,
                CreationDateTime = DateTime.Now
            });
        }
    }
}
