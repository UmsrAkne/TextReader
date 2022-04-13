namespace TextReader.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Prism.Mvvm;
    using TextReader.Models;
    using TextReader.Models.DBs;
    using TextReader.Models.Talkers;

    public class MainWindowViewModel : BindableBase
    {
        private readonly TextDBContext databaseContext = new TextDBContext(TextDBContext.CreateDbContextOptions());
        private readonly Player player = new Player();

        private string title = "Prism Application";
        private List<TitleRecord> titles = new List<TitleRecord>();
        private List<TextRecord> texts = new List<TextRecord>();

        private int selectionTitleIndex;

        public MainWindowViewModel()
        {
            databaseContext.Database.EnsureCreated();
            UpdateLists();
            player.Talker = new BouyomiTalker();
            player.Texts = Texts;
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public List<TextRecord> Texts { get => texts; set => SetProperty(ref texts, value); }

        public List<TitleRecord> Titles { get => titles; set => SetProperty(ref titles, value); }

        public int SelectionTitleIndex
        {
            get => selectionTitleIndex;
            set
            {
                SetProperty(ref selectionTitleIndex, value);
                Texts = databaseContext.GetTexts(Titles[SelectionTitleIndex].Id);
            }
        }

        /// <summary>
        /// 入力されたテキストをタイトルと共にデータベースに書き込みます。
        /// </summary>
        /// <param name="fileName">書き込みを行うテキストに紐付けられるタイトル</param>
        /// <param name="content">書き込むテキスト（行区切り）</param>
        public void WriteTextFile(string fileName, List<string> content)
        {
            /// Drag and Drop を受け付けるビヘイビアから呼び出されるメソッド

            int titleID = databaseContext.AddTitle(fileName);
            int counter = 0;
            var records = content.Select(t => new TextRecord()
            {
                Text = t,
                Index = counter++,
                TitleNumber = titleID,
                CreationDateTime = System.DateTime.Now
            }).ToList();

            databaseContext.AddTexts(records);
            UpdateLists();
        }

        private void UpdateLists()
        {
            Titles = databaseContext.GetTitles();

            if (Titles.Count > 0)
            {
                Texts = databaseContext.GetTexts(Titles.First().Id);
            }
        }
    }
}
