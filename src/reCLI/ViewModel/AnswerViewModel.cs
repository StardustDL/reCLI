using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using reCLI.Core;

namespace reCLI.ViewModel
{
    public class AnswerViewModel : NotifyPropertyChangedModel
    {
        public AnswerViewModel(Answer result,Guid from)
        {
            if (result != null)
            {
                Answer = result;
            }
            From = from;
        }

        public Guid From { get; set; }

        public Answer Answer { get; }

        public override bool Equals(object obj)
        {
            if (obj is AnswerViewModel r)
            {
                return Answer.Equals(r.Answer);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode() => Answer.GetHashCode();

        public override string ToString() => Answer.ToString();
    }
}
