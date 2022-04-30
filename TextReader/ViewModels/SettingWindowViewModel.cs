namespace TextReader.ViewModels
{
    using System;
    using Prism.Services.Dialogs;

    public class SettingWindowViewModel : IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public string Title => "Setting";

        public bool CanCloseDialog()
        {
            throw new NotImplementedException();
        }

        public void OnDialogClosed()
        {
            throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
