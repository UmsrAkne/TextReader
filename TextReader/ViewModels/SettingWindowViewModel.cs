namespace TextReader.ViewModels
{
    using System;
    using Prism.Services.Dialogs;

    public class SettingWindowViewModel : IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public string Title => "Setting";

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
