namespace TextReader.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TextRecord
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;
    }
}
