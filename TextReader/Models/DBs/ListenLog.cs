namespace TextReader.Models.DBs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ListenLog
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int TextID { get; set; }

        [Required]
        public int TalkerID { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
    }
}
