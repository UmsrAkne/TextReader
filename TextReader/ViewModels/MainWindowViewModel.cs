namespace TextReader.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Prism.Commands;
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
        private ObservableCollection<TextRecord> texts = new ObservableCollection<TextRecord>();

        private int selectionTitleIndex;
        private int selectionTextIndex;
        private bool playButtonEnabled = true;

        public MainWindowViewModel()
        {
            databaseContext.Database.EnsureCreated();
            UpdateLists();
            player.Talker = new BouyomiTalker();
            player.Texts = new List<TextRecord>(Texts);

            // player が読み上げを開始した際、テキストレコードの視聴回数のカウンターがインクリメントされる。
            // それをデータベースに反映させるため、イベントハンドラをセットする。
            player.PlayStarted += (sender, e) =>
            {
                PlayButtonEnabled = false;
                databaseContext.SaveChanges();
            };

            player.PlayStopped += (sender, e) => PlayButtonEnabled = true;
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ObservableCollection<TextRecord> Texts { get => texts; set => SetProperty(ref texts, value); }

        public List<TitleRecord> Titles { get => titles; set => SetProperty(ref titles, value); }

        public int SelectionTitleIndex
        {
            get => selectionTitleIndex;
            set
            {
                SetProperty(ref selectionTitleIndex, value);
                Texts = new ObservableCollection<TextRecord>(databaseContext.GetTexts(Titles[SelectionTitleIndex].Id));
            }
        }

        public int SelectionTextIndex { get => selectionTextIndex; set => SetProperty(ref selectionTextIndex, value); }

        public bool PlayButtonEnabled { get => playButtonEnabled; set => SetProperty(ref playButtonEnabled, value); }

        public DelegateCommand PlayCommand => new DelegateCommand(() =>
        {
            player.Play();
        });

        public DelegateCommand StopCommand => new DelegateCommand(() =>
        {
            player.Stop();
        });

        public DelegateCommand SetStartIndexCommand => new DelegateCommand(() =>
        {
            player.Index = SelectionTextIndex;
            player.Play();
        });

        public DelegateCommand RunBouyomiChanCommand => new DelegateCommand(() => Process.Start(@"BouyomiChan\BouyomiChan.exe"));

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
                Texts = new ObservableCollection<TextRecord>(databaseContext.GetTexts(Titles.First().Id));
            }
        }
    }
}
