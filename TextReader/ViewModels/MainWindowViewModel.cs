﻿namespace TextReader.ViewModels
{
    using System.Collections.Generic;
    using Prism.Mvvm;
    using TextReader.Models;
    using TextReader.Models.DBs;

    public class MainWindowViewModel : BindableBase
    {
        private readonly TextDBContext databaseContext = new TextDBContext(TextDBContext.CreateDbContextOptions());

        private string title = "Prism Application";

        private List<TitleRecord> titles = new List<TitleRecord>();

        private List<IText> texts = new List<IText>()
        {
            new PlainText("sampleText1"),
            new PlainText("sampleText2"),
            new PlainText("sampleText3")
        };

        public MainWindowViewModel()
        {
            databaseContext.Database.EnsureCreated();
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public List<IText> Texts { get => texts; set => SetProperty(ref texts, value); }

        public List<TitleRecord> Titles { get => titles; set => SetProperty(ref titles, value); }
    }
}
