namespace TextReader.ViewModels
{
    using System;
    using Prism.Commands;
    using Prism.Services.Dialogs;

    public class SettingWindowViewModel : IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public string Title => "Setting";

        public bool CanCloseDialog() => true;

        public DelegateCommand CloseWindowCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
