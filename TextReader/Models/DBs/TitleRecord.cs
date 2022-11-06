using System;
using System.ComponentModel.DataAnnotations;
using Prism.Mvvm;

namespace TextReader.Models.DBs
{
    public class TitleRecord : BindableBase
    {
        private string title = string.Empty;

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get => title; set => SetProperty(ref title, value); }

        [Required]
        public DateTime CreationDateTime { get; set; }
    }
}