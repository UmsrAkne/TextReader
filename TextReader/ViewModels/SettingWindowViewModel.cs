namespace TextReader.ViewModels
{
    using System;
    using System.Linq;
    using Prism.Commands;
    using Prism.Services.Dialogs;
    using TextReader.Models.DBs;
    using TextReader.Models.Talkers;

    public class SettingWindowViewModel : IDialogAware
    {
        private TextDBContext databaseContext;

        public event Action<IDialogResult> RequestClose;

        public BouyomiTalker BouyomiTalker { get; set; } = new BouyomiTalker();

        public AzureTalker AzureTalker { get; set; } = new AzureTalker();

        public string Title => "Setting";

        public bool CanCloseDialog() => true;

        public DelegateCommand CloseWindowCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public void OnDialogClosed()
        {
            void cloneSetting(ITalker talker, TalkerSetting destSetting)
            {
                destSetting.TalkerID = talker.TalkerID;
                destSetting.TalkSpeed = talker.TalkSpeed;
                destSetting.Volume = talker.Volume;
                destSetting.BlankLineWaitTime = talker.BlankLineWaitTime;
            }

            var bouyomiSetting = databaseContext.TalkerSettings.Where(bou => BouyomiTalker.TalkerID == bou.TalkerID).FirstOrDefault();

            if (bouyomiSetting != null)
            {
                cloneSetting(BouyomiTalker, bouyomiSetting);
            }
            else
            {
                bouyomiSetting = new TalkerSetting();
                cloneSetting(BouyomiTalker, bouyomiSetting);
                databaseContext.TalkerSettings.Add(bouyomiSetting);
            }

            var azureSetting = databaseContext.TalkerSettings.Where(az => AzureTalker.TalkerID == az.TalkerID).FirstOrDefault();

            if (azureSetting != null)
            {
                cloneSetting(AzureTalker, azureSetting);
            }
            else
            {
                azureSetting = new TalkerSetting();
                cloneSetting(AzureTalker, azureSetting);
                databaseContext.TalkerSettings.Add(azureSetting);
            }

            databaseContext.SaveChanges();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            databaseContext = parameters.GetValue<TextDBContext>(nameof(TextDBContext));
        }
    }
}
