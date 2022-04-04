namespace TextReader.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DBQueryer
    {
        public IEnumerable<TextRecord> Target { get; set; }

        public IEnumerable<TextRecord> GetTexts(string title, DateTime dateTime)
        {
            return Target
                .Where(record => title == record.Title && record.CreationDateTime == dateTime)
                .OrderBy(record => record.Index);
        }

        public void AddTexts(IEnumerable<TextRecord> texts)
        {
            Target.ToList().AddRange(texts);
        }
    }
}
