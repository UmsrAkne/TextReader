namespace TextReader.Models.DBs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TitleRecord
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime CreationDateTime { get; set; }
    }
}
