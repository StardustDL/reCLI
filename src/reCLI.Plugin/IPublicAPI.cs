using reCLI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace reCLI.Plugin
{
    public enum MessageIcon
    {
        Info,
        Success,
        Warning,
        Error
    }

    public interface IPublicAPI
    {
        /// <summary>
        /// Change reCLI query
        /// </summary>
        /// <param name="query">query text</param>
        /// <param name="requery">
        /// force requery By default, reCLI will not fire query if your query is same with existing one. 
        /// Set this to true to force reCLI requerying
        /// </param>
        void ChangeQuery(string query, bool requery = true);

        void PushAnswers(IEnumerable<Answer> answers);
        
        void ShowMessage(string title,TimeSpan timeSpan,MessageIcon icon = MessageIcon.Info);
        
        void ShowTip(string title, MessageIcon icon = MessageIcon.Info);

        void UIThreadWork(Action action);

        void HideTip();

        void Busying();

        void Unbusying();
    }
}
