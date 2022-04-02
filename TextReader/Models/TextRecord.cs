namespace TextReader.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TextRecord
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public int Index { get; set; }

        [Required]
        public bool Read { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime CreationDateTime { get; set; }
    }
}
