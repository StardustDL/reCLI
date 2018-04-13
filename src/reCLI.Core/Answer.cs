using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace reCLI.Core
{
    public class Answer : NotifyPropertyChangedModel
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string OriginalQuery { get; set; }

        public Func<ActionContext, Task<Result>> Execute { get; set; }

        public int Priority { get; set; }

        public override string ToString() => Title + SubTitle;

        /// <summary>
        /// Additional data associate with this result
        /// </summary>
        public object Content { get; set; }
    }
}
