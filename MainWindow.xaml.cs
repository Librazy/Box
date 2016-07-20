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
            var H = 4;
            var W = 5;
            InitializeComponent();
            MWindow.SourceInitialized += WinSourceInitialized;
            GameGrid.RowDefinitions.Clear(); 
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20,GridUnitType.Star) });
            for (int i = 0; i < H - 1; ++i) {
                GameGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star), MaxHeight = 15 });
                GameGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Star) });
            }
            GameGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star), MaxHeight = 15 });
            GameGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20,GridUnitType.Star) });

            GameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });
            for (int i = 0; i < W - 1; ++i)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star), MaxWidth = 15 });
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Star) });
            }
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star), MaxWidth = 15 });
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) });

            for (int i = 0; i < H; ++i)
            {
                for (int j = 0; j < W; ++j)
                {
                    var b = new Button() { Style = (Style)FindResource("DotButton"), };
                    Grid.SetRow(b, i*2+1);
                    Grid.SetColumn(b, j*2+1);
                    GameGrid.Children.Add(b);
                }
            }
            var rec = new Button() { Style = (Style)FindResource("LineButton"), };
            rec.Command = LineButtonClick;
            rec.MouseEnter += new MouseEventHandler(LineButtonOnME);
            rec.MouseLeave += new MouseEventHandler(LineButtonOnML);
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
        private static void SetBGTransform(Control b, Color from, Color to, double t, bool fg = false)
        {
            var brush = new SolidColorBrush();

            var colorAnimation = new ColorAnimation {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(t)),
                AutoReverse = false
            };

            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation, HandoffBehavior.Compose);
            if (fg)
            {
                b.Foreground = brush;
            }
            else
            {
                b.Background = brush;
            }
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

        #region MainWindow Events 

        private void LineButtonOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform((Button)sender, Color.FromRgb(78, 201, 178), Color.FromRgb(16, 81, 51), 0.1, true);
        }

        private void LineButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform((Button)sender, Color.FromRgb(16, 81, 51), Color.FromRgb(78, 201, 178), 0.3, true);
        }

        #endregion

        #region Commands

        private RelayCommand<Button> _lineButtonClick;

        public RelayCommand<Button> LineButtonClick 
            => _lineButtonClick ?? (_lineButtonClick = new RelayCommand<Button>((s)=>AddLine(s,Colors.Aqua)));

        public void AddLine(Button s,Color t)
        {
            s.Foreground = new SolidColorBrush(t);
        }

        #endregion
    }
}