using reCLI.Core;
using reCLI.Plugin;
using reCLI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace reCLI.ViewModel
{
    public class TipViewModel : NotifyPropertyChangedModel
    {
        

        public TipViewModel()
        {
            
        }

        public ImageSource Icon { get; private set; }

        public string Text { get; set; }

        public void SetIcon(MessageIcon icon)
        {
            switch (icon)
            {
                case MessageIcon.Info:
                    Icon = Images.Infomation;
                    break;
                case MessageIcon.Success:
                    Icon = Images.Success;
                    break;
                case MessageIcon.Warning:
                    Icon = Images.Warning;
                    break;
                case MessageIcon.Error:
                    Icon = Images.Error;
                    break;
                default:
                    break;
            }
        }
    }
}
