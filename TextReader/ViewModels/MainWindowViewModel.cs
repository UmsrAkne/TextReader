namespace TextReader.ViewModels
{
    using System.Collections.Generic;
    using Prism.Mvvm;
    using TextReader.Models;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private readonly TextDBContext databaseContext = new TextDBContext();

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
    }
}
