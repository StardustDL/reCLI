using reCLI.Core;
using reCLI.Infrastructure.Hotkey;
using reCLI.Plugin;
using reCLI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace reCLI
{
    class PublicAPIInstance : IPublicAPI
    {
        public Guid PluginGuid { get; private set; }

        public PublicAPIInstance(Guid pluginGuid)
        {
            PluginGuid = pluginGuid;
        }

        public void ChangeQuery(string query, bool requery = true)
        {
            MainViewModel.Current.ChangeQueryText(query, requery);
        }

        public void ShowMessage(string title, TimeSpan timeSpan, MessageIcon icon = MessageIcon.Info)
        {
            MainViewModel.Current.ShowTip(title,icon);
            Task.Delay(timeSpan).ContinueWith(_ => MainViewModel.Current.HideTip());
        }

        public void ShowTip(string title, MessageIcon icon = MessageIcon.Info)
        {
            MainViewModel.Current.ShowTip(title, icon);
        }

        public void HideTip()
        {
            MainViewModel.Current.HideTip();
        }

        public void Busying()
        {
            MainViewModel.Current.ShowProgressBar();
        }

        public void Unbusying()
        {
            MainViewModel.Current.HideProgressBar();
        }

        public void PushAnswers(IEnumerable<Answer> answers)
        {
            MainViewModel.Current.PushAnswers(answers, PluginGuid);
        }

        public void UIThreadWork(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }
    }
}
