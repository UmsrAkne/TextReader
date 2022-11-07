using System;
using System.Linq;
using Prism.Commands;
using Prism.Services.Dialogs;
using TextReader.Models.DBs;
using TextReader.Models.Talkers;

namespace TextReader.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SettingWindowViewModel : IDialogAware
    {
        private TextDbContext databaseContext;

        public event Action<IDialogResult> RequestClose;

        public BouyomiTalker BouyomiTalker { get; private set; } = new BouyomiTalker();

        public AzureTalker AzureTalker { get; private set; } = new AzureTalker();

        public string Title => "Setting";

        public DelegateCommand CloseWindowCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            void CloneSetting(ITalker talker, TalkerSetting destSetting)
            {
                destSetting.TalkerID = talker.TalkerID;
                destSetting.TalkSpeed = talker.TalkSpeed;
                destSetting.Volume = talker.Volume;
                destSetting.BlankLineWaitTime = (int)talker.BlankLineWaitTime.TotalMilliseconds;
            }

            var bouyomiSetting = databaseContext.TalkerSettings.FirstOrDefault(bou => BouyomiTalker.TalkerID == bou.TalkerID);

            if (bouyomiSetting != null)
            {
                CloneSetting(BouyomiTalker, bouyomiSetting);
                bouyomiSetting.BouyomiChanDirectoryPath = BouyomiTalker.BouyomiChanLocation;
            }
            else
            {
                databaseContext.TalkerSettings.Add(BouyomiTalker.Setting);
            }

            var azureSetting = databaseContext.TalkerSettings.FirstOrDefault(az => AzureTalker.TalkerID == az.TalkerID);

            if (azureSetting != null)
            {
                CloneSetting(AzureTalker, azureSetting);
                azureSetting.AzureTTSKeyVariableName = AzureTalker.SecretKeyVariableName; 
            }
            else
            {
                databaseContext.TalkerSettings.Add(AzureTalker.Setting);
            }

            databaseContext.SaveChanges();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            databaseContext = parameters.GetValue<TextDbContext>(nameof(TextDbContext));

            var bouyomiSetting = databaseContext.TalkerSettings.FirstOrDefault(bou => BouyomiTalker.TalkerID == bou.TalkerID);

            if (bouyomiSetting != null)
            {
                BouyomiTalker = new BouyomiTalker() { Setting = bouyomiSetting };
            }

            var azureSetting = databaseContext.TalkerSettings.FirstOrDefault(az => AzureTalker.TalkerID == az.TalkerID);

            if (azureSetting != null)
            {
                AzureTalker = new AzureTalker() { Setting = azureSetting };
            }
        }
    }
}