namespace TextReader.Models.DBs
{
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
        public string AzureTTSKeyVariableName { get; set; } = "Microsoft_Speech_Secret_key";

        [Required]
        public string BouyomiChanDirectoryPath { get; set; } = @"BouyomiChan\RemoteTalk\RemoteTalk.exe";

        [Required]
        public int BlankLineWaitTime { get; set; } = 3000;
    }
}
