namespace TextReader.Models.DBs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TalkerSetting
    {
        [Key]
        [Required]
        public int TalkerID { get; set; }

        [Required]
        public int TalkSpeed { get; set; }

        [Required]
        public int Volume { get; set; }

        [Required]
        public string AzureTTSKeyVariableName { get; set; } = string.Empty;

        [Required]
        public string BouyomiChanDirectoryPath { get; set; } = string.Empty;

        [Required]
        public TimeSpan BlankLineWaitTime { get; set; }
    }
}
