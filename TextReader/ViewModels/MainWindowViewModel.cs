namespace TextReader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Controls;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using TextReader.Models;
    using TextReader.Models.DBs;
    using TextReader.Models.Talkers;
    using TextReader.Views;

    public class MainWindowViewModel : BindableBase
    {
        private readonly TextDBContext databaseContext = new TextDBContext(TextDBContext.CreateDbContextOptions());
        private readonly Player player = new Player();

        private string title = "Text reader";
        private List<TitleRecord> titles = new List<TitleRecord>();
        private ObservableCollection<TextRecord> texts = new ObservableCollection<TextRecord>();

        private int selectionTitleIndex;
        private int selectionTextIndex;
        private bool playing = false;
        private int playingIndex;
        private int readCharacterCount;
        private ITalker talker;
        private IDialogService dialogService;
        private double readRatio = 0;

        public MainWindowViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            databaseContext.Database.EnsureCreated();
            UpdateLists();
            ChangeTalkerCommand.Execute(new BouyomiTalker());
            player.Texts = new List<TextRecord>(Texts);

            // player が読み上げを開始した際、テキストレコードの視聴回数のカウンターがインクリメントされる。
            // それをデータベースに反映させるため、イベントハンドラをセットする。
            player.PlayStarted += (sender, e) =>
            {
                Playing = true;
                PlayingIndex = player.Index;
                SelectionTextIndex = PlayingIndex;

                databaseContext.Add(new ListenLog()
                {
                    TextID = player.Texts[PlayingIndex].Id,
                    TalkerID = player.Talker.TalkerID,
                    DateTime = System.DateTime.Now
                });

                ReadCharacterCount += player.Texts[PlayingIndex].Text.Length;

                databaseContext.SaveChanges();
            };

            player.PlayStopped += (sender, e) =>
            {
                PlayingIndex = player.Index;
                Playing = false;
                ReadRatio = PlayingIndex != 0 ? Math.Round(100.0 * PlayingIndex / player.Texts.Count, 3) : 0;
            };
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
                player.Texts = new List<TextRecord>(Texts);
            }
        }

        public int SelectionTextIndex { get => selectionTextIndex; set => SetProperty(ref selectionTextIndex, value); }

        /// <summary>
        /// Player の中にも talker プロパティがあり、通常はそちらを使う。
        /// こっちの方のプロパティは、ビューに表示する情報を取り出すために使う。(インスタンスは上述のものと同一)
        /// テストの関係で Player が BindableBase を継承できなかったため、２つに分かれている。
        /// </summary>
        public ITalker Talker { get => talker; set => SetProperty(ref talker, value); }

        public int ReadCharacterCount { get => readCharacterCount; set => SetProperty(ref readCharacterCount, value); }

        public bool Playing { get => playing; set => SetProperty(ref playing, value); }

        public int PlayingIndex { get => playingIndex; set => SetProperty(ref playingIndex, value); }

        public double ReadRatio { get => readRatio; set => SetProperty(ref readRatio, value); }

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

        public DelegateCommand<ListView> ScrollCommand => new DelegateCommand<ListView>((lv) =>
        {
            if (lv.Items.Count != 0 && SelectionTextIndex >= 0)
            {
                lv.ScrollIntoView(lv.Items.GetItemAt(SelectionTextIndex));
            }
        });

        public DelegateCommand RunBouyomiChanCommand => new DelegateCommand(() => Process.Start(@"BouyomiChan\BouyomiChan.exe"));

        public DelegateCommand<ITalker> ChangeTalkerCommand => new DelegateCommand<ITalker>((paramTalker) =>
        {
            /// 下記のような経路で引数が回ってくる
            /// CommandParameter として、MainWindow.xaml の方で生成した ITalker のインスタンスが入力される
            /// このクラスのコンストラクタで初期設定を行う際に ITalker が入力される。
            /// 設定画面で行った設定を反映させるため、MainWindowViewModel が保持している現在選択中の ITalker が入力される

            /// TalkerID を使ってデータベースから TalkerSetting を検索する
            if (databaseContext.TalkerSettings.Any(ts => ts.TalkerID == paramTalker.TalkerID))
            {
                paramTalker.Setting = databaseContext.TalkerSettings.Where(ts => ts.TalkerID == paramTalker.TalkerID).First();
            }

            Talker = paramTalker;
            player.Talker = Talker;

            ReadCharacterCount = databaseContext.Histories
                .Where(l => l.TalkerID == Talker.TalkerID)
                .Select(bl => databaseContext.GetText(bl.TextID).Text.Length).ToList()
                .Sum();
        });

        public DelegateCommand ShowSettingWindowCommand => new DelegateCommand(() =>
        {
            var param = new DialogParameters();
            param.Add(nameof(TextDBContext), databaseContext);
            dialogService.ShowDialog(nameof(SettingWindow), param, new Action<IDialogResult>(r => { }));
            ChangeTalkerCommand.Execute(Talker);
        });

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
                player.Texts = new List<TextRecord>(Texts);
            }
        }
    }
}
