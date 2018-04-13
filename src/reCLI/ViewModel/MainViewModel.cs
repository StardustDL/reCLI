using NHotkey;
using NHotkey.Wpf;
using reCLI.Core;
using reCLI.Helpers;
using reCLI.Infrastructure;
using reCLI.Infrastructure.Hotkey;
using reCLI.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace reCLI.ViewModel
{
    public class MainViewModel : NotifyPropertyChangedModel
    {
        internal static MainViewModel Current { get; private set; }

        #region Private Fields
        
        private Query _lastQuery = new Query();

        private CancellationTokenSource _updateSource;
        private CancellationToken _updateToken;

        private IEnumerable<(IGlobalQuery, PluginMetadata)> globalPlugin;

        #endregion
        
        #region Constructor

        public MainViewModel()
        {
            Current = this;
            
            Answers = new AnswersViewModel();
            Tip = new TipViewModel();
            globalPlugin = App.PluginManager.GetPluginsForFeature<IGlobalQuery>();
            TipVisibility = Visibility.Collapsed;

#if DEBUG
#else
            SetHotkey("Alt + Space", OnHotkey);
            SetCustomPluginHotkey();
#endif
        }

#endregion

#region ViewModel Properties

        public AnswersViewModel Answers { get; private set; }

        public TipViewModel Tip { get; private set; }

        private string _queryText;

        public string QueryText
        {
            get => _queryText;
            set
            {
                _queryText = value;
                Query();
            }
        }

        public bool LastQuerySelected { get; set; }

        public bool QueryTextCursorMovedToEnd { get; set; }

        public Visibility ProgressBarVisibility { get; set; }

        public Visibility TipVisibility { get; set; }

        public Visibility MainWindowVisibility { get; set; }

#endregion

#region Commands

        ICommand _escCommand, _selectNextItemCommand, _selectPrevItemCommand, _selectNextPageCommand, _selectPrevPageCommand,_openAnswerCommand,_requeryCommand;

        public ICommand EscCommand
        {
            get
            {
                if (_escCommand == null) _escCommand = new RelayCommand(() =>
                    {
                        MainWindowVisibility = Visibility.Collapsed;
                    });
                return _escCommand;
            }
        }

        public ICommand SelectNextItemCommand
        {
            get
            {
                if (_selectNextItemCommand == null) _selectNextItemCommand = new RelayCommand(() => Answers.NextAnswer());
                return _selectNextItemCommand;
            }
        }

        public ICommand SelectPrevItemCommand
        {
            get
            {
                if (_selectPrevItemCommand == null) _selectPrevItemCommand = new RelayCommand(() => Answers.PrevAnswer());
                return _selectPrevItemCommand;
            }
        }
        public ICommand SelectNextPageCommand
        {
            get
            {
                if (_selectNextPageCommand == null) _selectNextPageCommand = new RelayCommand(() => Answers.NextPage());
                return _selectNextPageCommand;
            }
        }
        public ICommand SelectPrevPageCommand
        {
            get
            {
                if (_selectPrevPageCommand == null) _selectPrevPageCommand = new RelayCommand(() => Answers.PrevPage());
                return _selectPrevPageCommand;
            }
        }

        public ICommand RequeryCommand
        {
            get
            {
                if (_requeryCommand == null) _requeryCommand = new RelayCommand(() =>Query());
                return _requeryCommand;
            }
        }
        public ICommand StartHelpCommand { get; set; }

        public ICommand OpenAnswerCommand
        {
            get
            {
                if (_openAnswerCommand == null) _openAnswerCommand = new RelayCommand<int?>(async index =>
                      {
                          var results = Answers;

                          if (index.HasValue)
                          {
                              results.SelectedIndex = index.Value;
                          }

                          var result = results.SelectedItem?.Answer;
                          if (result != null) // SelectedItem returns null if selection is empty.
                          {
                              if(result.OriginalQuery!=null)ChangeQueryText(result.OriginalQuery, false);
                              if (result.Execute != null)
                              {
                                  var res = await result.Execute(new ActionContext
                                  {
                                      SpecialKeyState = GlobalHotkey.Instance.CheckModifiers()
                                  });

                                  if (res == null || res.AutoHide)
                                  {
                                      MainWindowVisibility = Visibility.Collapsed;
                                  }
                              }
                              else//Not a real call
                              {

                              }
                          }
                      });
                return _openAnswerCommand;
            }
        }

#endregion

        static readonly Thickness AnswerViewOpenMargin = new Thickness { Top = 8 };
        static readonly Thickness AnswerViewCloseMargin = new Thickness();

        /// <summary>
        /// we need move cursor to end when we manually changed query
        /// but we don't want to move cursor to end when query is updated from TextBox
        /// </summary>
        /// <param name="queryText"></param>
        public void ChangeQueryText(string queryText, bool requery = true)
        {
            QueryTextCursorMovedToEnd = true;
            if (requery)
            {
                QueryText = queryText;
            }
            else
            {
                _queryText = queryText;
                base.OnPropertyChanged(nameof(QueryText));
            }
        }

        public void ShowTip(string text, MessageIcon icon = MessageIcon.Info)
        {
            Tip.Text = text;
            Tip.SetIcon(icon);
            TipVisibility = Visibility.Visible;
        }

        public void HideTip()
        {
            TipVisibility = Visibility.Collapsed;
        }

        public void ShowProgressBar()
        {
            ProgressBarVisibility = Visibility.Visible;
        }

        public void HideProgressBar()
        {
            ProgressBarVisibility = Visibility.Hidden;
        }

        public void PushAnswers(IEnumerable<Answer> answers,Guid id)
        {
            _updateSource?.Cancel();
            Answers.Answers.StartRefresh();
            foreach (var v in answers) Answers.AddAnswer(v, id);
            Answers.Answers.EndRefresh();
        }

        private async Task Query()
        {
            if (!string.IsNullOrEmpty(QueryText))
            {
                _updateSource?.Cancel();
                _updateSource = new CancellationTokenSource();
                _updateToken = _updateSource.Token;
                Answers.Visbility = Visibility.Visible;
                Answers.Margin = AnswerViewOpenMargin;
                HideProgressBar();
                var query = App.PluginManager.ParseQuery(QueryText);
                if (query != null)
                {
                    _lastQuery = query;
                    ShowProgressBar();

                    var plugins = App.PluginManager.GetValidPlugins(query);

                    Answers.Answers.StartRefresh();
                    await Task.WhenAll(plugins.Select(plugin => Task.Run(async () =>
                    {
                        var results = await plugin.Item1.Query(query, _updateToken);
                        foreach (var v in results) Answers.AddAnswer(v, plugin.Item2.ID);
                        if (/*Answers.Visbility != Visibility.Visible && */Answers.Answers.Count > 0)
                        {
                            Answers.SelectedIndex = 0;
                        }
                    },_updateToken)));

                    if (_updateToken.IsCancellationRequested) return;

                    await Task.WhenAll(globalPlugin.Select(plugin => Task.Run(async () =>
                    {
                        var results = await plugin.Item1.GlobalQuery(query, _updateToken);
                        foreach (var v in results) Answers.AddAnswer(v, plugin.Item2.ID);
                        if (/*Answers.Visbility != Visibility.Visible && */Answers.Answers.Count > 0)
                        {
                            Answers.SelectedIndex = 0;
                        }
                    }, _updateToken)));


                    if (_updateToken.IsCancellationRequested) return;

                    /*Parallel.ForEach(globalPlugin, plugin =>
                    {
                        var results = plugin.Item1.GlobalQuery(query).Result;
                        foreach (var v in results) Answers.AddAnswer(v, plugin.Item2.ID);
                        if (
                    Answers.Answers.Count > 0)
                        {
                            Answers.SelectedIndex = 0;
                        }
                    });*/
                    Answers.Answers.EndRefresh();
                    var list = Answers.Answers.OrderByDescending(x => x.Answer?.Priority ?? 0);
                    Answers.Answers.StartRefresh();
                    foreach (var v in list) Answers.AddAnswer(v);
                    Answers.Answers.EndRefresh();
                    HideProgressBar();
                }
            }
            else
            {
                Answers.Clear();
                Answers.Visbility = Visibility.Collapsed;
                Answers.Margin = AnswerViewCloseMargin;
            }
        }
        
#region Hotkey

        private void SetHotkey(string hotkeyStr, EventHandler<HotkeyEventArgs> action)
        {
            var hotkey = new HotkeyModel(hotkeyStr);
            SetHotkey(hotkey, action);
        }

        private void SetHotkey(HotkeyModel hotkey, EventHandler<HotkeyEventArgs> action)
        {
            string hotkeyStr = hotkey.ToString();
            try
            {
                HotkeyManager.Current.AddOrReplace(hotkeyStr, hotkey.CharKey, hotkey.ModifierKeys, action);
            }
            catch (Exception)
            {
                string errorMsg =
                    string.Format("注册热键失败：{0}", hotkeyStr);
                MessageBox.Show(errorMsg);
            }
        }

        public void RemoveHotkey(string hotkeyStr)
        {
            if (!string.IsNullOrEmpty(hotkeyStr))
            {
                HotkeyManager.Current.Remove(hotkeyStr);
            }
        }

        /// <summary>
        /// Checks if Wox should ignore any hotkeys
        /// </summary>
        /// <returns></returns>
        private bool ShouldIgnoreHotkeys()
        {
            //double if to omit calling win32 function
            /*if (_settings.IgnoreHotkeysOnFullscreen)
                if (WindowsInteropHelper.IsWindowFullscreen())
                    return true;*/

            return false;
        }

        private void SetCustomPluginHotkey()
        {
            return;
            /*if (_settings.CustomPluginHotkeys == null) return;
            foreach (CustomPluginHotkey hotkey in _settings.CustomPluginHotkeys)
            {
                SetHotkey(hotkey.Hotkey, (s, e) =>
                {
                    if (ShouldIgnoreHotkeys()) return;
                    MainWindowVisibility = Visibility.Visible;
                    ChangeQueryText(hotkey.ActionKeyword);
                });
            }*/
        }

        private void OnHotkey(object sender, HotkeyEventArgs e)
        {
            if (!ShouldIgnoreHotkeys())
            {

                /*if (_settings.LastQueryMode == LastQueryMode.Empty)
                {
                    ChangeQueryText(string.Empty);
                }
                else if (_settings.LastQueryMode == LastQueryMode.Preserved)
                {
                    LastQuerySelected = true;
                }
                else if (_settings.LastQueryMode == LastQueryMode.Selected)
                {
                    LastQuerySelected = false;
                }
                else
                {
                    throw new ArgumentException($"wrong LastQueryMode: <{_settings.LastQueryMode}>");
                }*/
                LastQuerySelected = true;

                Toggle();
                e.Handled = true;
            }
        }

        private void Toggle()
        {
            if (MainWindowVisibility != Visibility.Visible)
            {
                MainWindowVisibility = Visibility.Visible;
            }
            else
            {
                MainWindowVisibility = Visibility.Collapsed;
            }
        }

#endregion
    }
}
