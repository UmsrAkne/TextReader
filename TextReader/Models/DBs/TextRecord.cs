namespace TextReader.Models.DBs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Prism.Mvvm;

    public class TextRecord : BindableBase 
    {
        private bool isPlaying;

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
        public int TitleNumber { get; set; }

        [Required]
        public int ListenCount { get; set; }

        [Required]
        public DateTime CreationDateTime { get; set; }

        [NotMapped]
        public bool IsPlaying { get => isPlaying; set => SetProperty(ref isPlaying, value); }
    }
}
