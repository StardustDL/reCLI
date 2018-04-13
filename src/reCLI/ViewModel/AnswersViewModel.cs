using reCLI.Core;
using reCLI.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace reCLI.ViewModel
{
    public class RefreshableCollection<T> : ObservableCollection<T>
    {
        public bool IsRefreshing { get; private set; }

        int CurrentPosition;

        public void StartRefresh()
        {
            IsRefreshing = true;
            CurrentPosition = 0;
        }

        public void EndRefresh()
        {
            if (!IsRefreshing) return;
            while (CurrentPosition < Count) RemoveAt(Count - 1);
            IsRefreshing = false;
            CurrentPosition = 0;
        }

        public void RefreshAppend(T item)
        {
            if (!IsRefreshing) throw new InvalidOperationException("Not refresh");
            if (CurrentPosition < Count) this[CurrentPosition] = item;
            else this.Add(item);
            CurrentPosition++;
        }
    }

    public class AnswersViewModel : NotifyPropertyChangedModel
    {
        #region Private Fields

        public RefreshableCollection<AnswerViewModel> Answers { get; set; }

        private readonly object _addResultsLock = new object();
        private readonly object _collectionLock = new object();
        private int MaxAnswers => 6;

        public AnswersViewModel()
        {
            Answers = new RefreshableCollection<AnswerViewModel>();
            BindingOperations.EnableCollectionSynchronization(Answers, _collectionLock);
        }

        #endregion

        #region Properties

        public int MaxHeight => MaxAnswers * 50;

        public int SelectedIndex { get; set; }

        public AnswerViewModel SelectedItem { get; set; }
        public Thickness Margin { get; set; }
        public Visibility Visbility { get; set; } = Visibility.Collapsed;

        #endregion

        #region Private Methods

        private int MaintainIndex(int i)
        {
            var n = Answers.Count;
            if (n > 0)
            {
                i = (n + i) % n;
                return i;
            }
            else
            {
                return -1;
            }
        }

        #endregion

        public void NextAnswer()
        {
            SelectedIndex = MaintainIndex(SelectedIndex + 1);
        }

        public void PrevAnswer()
        {
            SelectedIndex = MaintainIndex(SelectedIndex - 1);
        }

        public void NextPage()
        {
            SelectedIndex = MaintainIndex(SelectedIndex + MaxAnswers);
        }

        public void PrevPage()
        {
            SelectedIndex = MaintainIndex(SelectedIndex - MaxAnswers);
        }

        public void Clear()
        {
            Answers.Clear();
        }

        /// <summary>
        /// To avoid deadlock, this method should not called from main thread
        /// </summary>
        public void AddAnswer(Answer answer,Guid from)
        {
            lock (_addResultsLock)
            {
                Answers.RefreshAppend(new AnswerViewModel(answer, from));
            }
        }

        /// <summary>
        /// To avoid deadlock, this method should not called from main thread
        /// </summary>
        public void AddAnswer(AnswerViewModel answer)
        {
            lock (_addResultsLock)
            {
                Answers.RefreshAppend(answer);
            }
        }
    }
}
