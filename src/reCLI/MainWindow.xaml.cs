using reCLI.Helpers;
using reCLI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace reCLI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private readonly Storyboard _progressBarStoryboard = new Storyboard();

        #endregion
        
        public MainWindow()
        {
            DataContext = MainViewModel.Current;
            InitializeComponent();
            
        }

        private void OnLoaded(object sender, RoutedEventArgs _)
        {
            // todo is there a way to set blur only once?
            BlurWindowHelper.SetBlurForWindow();
            WindowsInteropHelper.DisableControlBox(this);
            //InitProgressbarAnimation();
            InitializePosition();
            // since the default main window visibility is visible
            // so we need set focus during startup
            QueryTextBox.Focus();

            MainViewModel.Current.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.MainWindowVisibility))
                {
                    if (Visibility == Visibility.Visible)
                    {
                        Activate();
                        QueryTextBox.Focus();
                        UpdatePosition();
                        if (MainViewModel.Current.LastQuerySelected)
                        {
                            QueryTextBox.SelectAll();
                            MainViewModel.Current.LastQuerySelected = false;
                        }
                    }
                }
            };
            InitializePosition();
        }

        private void InitializePosition()
        {
            Top = WindowTop();
            Left = WindowLeft();
        }

        /*private void InitProgressbarAnimation()
        {
            var da = new DoubleAnimation(ProgressBar.X2, ActualWidth + 100, new Duration(new TimeSpan(0, 0, 0, 0, 1600)));
            var da1 = new DoubleAnimation(ProgressBar.X1, ActualWidth, new Duration(new TimeSpan(0, 0, 0, 0, 1600)));
            Storyboard.SetTargetProperty(da, new PropertyPath("(Line.X2)"));
            Storyboard.SetTargetProperty(da1, new PropertyPath("(Line.X1)"));
            _progressBarStoryboard.Children.Add(da);
            _progressBarStoryboard.Children.Add(da1);
            _progressBarStoryboard.RepeatBehavior = RepeatBehavior.Forever;
            ProgressBar.BeginStoryboard(_progressBarStoryboard);
        }*/

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && e.OriginalSource != null)
            {
                var r = (AnswerListBox)sender;
                var d = (DependencyObject)e.OriginalSource;
                var item = ItemsControl.ContainerFromElement(r, d) as ListBoxItem;
                var result = (AnswerViewModel)item?.DataContext;
                if (result != null)
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        MainViewModel.Current.OpenAnswerCommand.Execute(Core.InvokeKey.Enter);
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                    {
                        //MainViewModel.Current.LoadContextMenuCommand.Execute(null);
                    }
                }
            }
        }
        
        private void OnDeactivated(object sender, EventArgs e)
        {
#if DEBUG
      
#else
            Hide();
#endif
        }

        private void UpdatePosition()
        {
            Left = WindowLeft();
            Top = WindowTop();
        }

        private double WindowLeft()
        {
            var screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            var dip1 = WindowsInteropHelper.TransformPixelsToDIP(this, screen.WorkingArea.X, 0);
            var dip2 = WindowsInteropHelper.TransformPixelsToDIP(this, screen.WorkingArea.Width, 0);
            var left = (dip2.X - ActualWidth) / 2 + dip1.X;
            return left;
        }

        private double WindowTop()
        {
            var screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            var dip1 = WindowsInteropHelper.TransformPixelsToDIP(this, 0, screen.WorkingArea.Y);
            var dip2 = WindowsInteropHelper.TransformPixelsToDIP(this, 0, screen.WorkingArea.Height);
            var top = (dip2.Y - QueryTextBox.ActualHeight) / 4 + dip1.Y;
            return top;
        }
        
        /// <summary>
        /// Register up and down key
        /// todo: any way to put this in xaml ?
        /// </summary>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                MainViewModel.Current.SelectNextItemCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                MainViewModel.Current.SelectPrevItemCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.PageDown)
            {
                MainViewModel.Current.SelectNextPageCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.PageUp)
            {
                MainViewModel.Current.SelectPrevPageCommand.Execute(null);
                e.Handled = true;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainViewModel.Current.QueryTextCursorMovedToEnd)
            {
                QueryTextBox.CaretIndex = QueryTextBox.Text.Length;
                MainViewModel.Current.QueryTextCursorMovedToEnd = false;
            }
        }
    }
}
