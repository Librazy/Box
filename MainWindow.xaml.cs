using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.CommandWpf;

namespace LiCalculatorWPF
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        #region Core Function
        public MainWindow()
        {
            InitializeComponent();
            MWindow.SourceInitialized += WinSourceInitialized;
            var rec = new Button() { Style = (Style)FindResource("LineButton"), };
            rec.Command = LineButtonClick;
            Grid.SetRow(rec,3);
            Grid.SetColumn(rec, 2);
            GameGrid.Children.Add(rec);
        }
        /// <summary>
        ///     设置一个控件的背景色渐变
        /// </summary>
        /// <param name="b">控件</param>
        /// <param name="from">起始颜色</param>
        /// <param name="to">终止颜色</param>
        /// <param name="t">时间 秒</param>
        private static void SetBGTransform(Control b, Color from, Color to, double t)
        {
            var brush = new SolidColorBrush();

            var colorAnimation = new ColorAnimation {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(t)),
                AutoReverse = false
            };

            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation, HandoffBehavior.Compose);
            b.Background = brush;
        }
        #endregion

        #region Dependency Property Core

        /// <summary>
        ///     主窗体的外边距（阴影宽度）
        /// </summary>
        public int WMargin
        {
            get { return (int) GetValue(WMarginProperty); }
            set { SetValue(WMarginProperty, value); }
        }

        /// <summary>
        ///     主窗体的外边距（阴影宽度）的DependencyProperty
        /// </summary>
        // Using a DependencyProperty as the backing store for WMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WMarginProperty =
            DependencyProperty.Register("WMargin", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public double ScaledFontSize
        {
            get { return (double) GetValue(ScaledFontSizeProperty); }
            set { SetValue(ScaledFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaledFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaledFontSizeProperty =
            DependencyProperty.Register("ScaledFontSize", typeof(double), typeof(MainWindow), new PropertyMetadata(30d));
        #endregion

        #region Dependency Property
        #endregion

        #region MainWindow Events Core

        private void MWindowSC(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) {
                MaximizeAndRestoreButton.Content = "";
                WMargin = 0;
            } else {
                MaximizeAndRestoreButton.Content = "";
                WMargin = 10;
            }
            //ScaledFontSize = (BackspaceButton?.FontSize + 24)/2 ?? 30;
        }

        private void CloseButtonOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform(CloseButton, Color.FromRgb(242, 242, 242), Colors.Red, 0.1);
        }

        private void CloseButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform(CloseButton, Colors.Red, Color.FromRgb(242, 242, 242), 0.3);
        }

        private void MARButtonOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform(MaximizeAndRestoreButton, Color.FromRgb(242, 242, 242), Color.FromRgb(218, 218, 218), 0.1);
        }

        private void MARButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform(MaximizeAndRestoreButton, Color.FromRgb(218, 218, 218), Color.FromRgb(242, 242, 242), 0.3);
        }

        private void MiniumButtonOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform(MiniumButton, Color.FromRgb(242, 242, 242), Color.FromRgb(218, 218, 218), 0.1);
        }

        private void MiniumButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform(MiniumButton, Color.FromRgb(218, 218, 218), Color.FromRgb(242, 242, 242), 0.3);
        }

        private void TitlebarOnMD(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MiniumButtonOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MARButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
                MaximizeAndRestoreButton.Content = "";
            } else {
                WindowState = WindowState.Maximized;
                MaximizeAndRestoreButton.Content = "";
            }
        }

        private void CloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        #endregion

        #region Commands

        private RelayCommand<Button> _lineButtonClick;

        public RelayCommand<Button> LineButtonClick => _lineButtonClick ??
                                                        (_lineButtonClick = new RelayCommand<Button>((s)=>AddLine(s,Colors.Aqua)));

        public void AddLine(Button s,Color t)
        {
            s.Foreground = new SolidColorBrush(t);
        }

        #endregion
    }
}