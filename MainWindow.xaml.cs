using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.CommandWpf;
using DotsAndBoxes.Dot;
namespace DotsAndBoxes
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
            SetGrid(GameGrid, 4, 5);
            SetDot(GameGrid, 4, 5);
            SetLine(GameGrid, 4, 5);
            Gaming.GridColorChanged
                += SetGridColor;
            Gaming.Init(4, 5);
        }

        /// <summary>
        ///     设置一个控件的背景色渐变
        /// </summary>
        /// <param name="b">控件</param>
        /// <param name="from">起始颜色</param>
        /// <param name="to">终止颜色</param>
        /// <param name="t">时间 秒</param>
        /// <param name="fg">是否设置前景色</param>
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
            if (fg) {
                b.Foreground = brush;
            } else {
                b.Background = brush;
            }
        }

        #endregion

        #region Gaming Function

        private void SetLine(Grid g, int h, int w)
        {
            //横向
            for (var i = 0; i < h ; ++i) {
                for (var j = 0; j < w - 1; ++j) {
                    var rec = new Button {
                        Style = (Style) FindResource("LineButton"),
                        BorderThickness = new Thickness(0, 5, 0, 5),
                        Command = LineButtonClick
                    };
                    rec.MouseEnter += LineButtonOnME;
                    rec.MouseLeave += LineButtonOnML;
                    Grid.SetRow(rec, 1 + i * 2);
                    Grid.SetColumn(rec, 2 + j * 2);
                    g.Children.Add(rec);
                }
            }
            //纵向
            for (var i = 0; i < h - 1; ++i)
            {
                for (var j = 0; j < w ; ++j)
                {
                    var rec = new Button
                    {
                        Style = (Style)FindResource("LineButton"),
                        BorderThickness = new Thickness(5,0,5,0),
                        Command = LineButtonClick
                    };
                    rec.MouseEnter += LineButtonOnME;
                    rec.MouseLeave += LineButtonOnML;
                    Grid.SetRow(rec, 2 + i * 2);
                    Grid.SetColumn(rec, 1 + j * 2);
                    g.Children.Add(rec);
                }
            }
        }

        private void SetDot(Grid g, int h, int w)
        {
            for (var i = 0; i < h; ++i)
            {
                for (var j = 0; j < w; ++j)
                {
                    var b = new Button { Style = (Style)FindResource("DotButton") };
                    Grid.SetRow(b, i * 2 + 1);
                    Grid.SetColumn(b, j * 2 + 1);
                    g.Children.Add(b);
                }
            }
        }
        
        private void SetGrid(Grid g, int h, int w)
        {
            g.RowDefinitions.Clear();
            g.ColumnDefinitions.Clear();
            g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) });
            for (var i = 0; i < h - 1; ++i)
            {
                g.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(10, GridUnitType.Star),
                    MaxHeight = 15
                });
                g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Star) });
            }
            g.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(10, GridUnitType.Star),
                MaxHeight = 15
            });
            g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) });

            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            for (var i = 0; i < w - 1; ++i)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(10, GridUnitType.Star),
                    MaxWidth = 15
                });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50, GridUnitType.Star) });
            }
            g.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(10, GridUnitType.Star),
                MaxWidth = 15
            });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
        }

        private void SetGridColor(int h, int w)
        {
            var i = h*2;
            var j = w*2;
            var b = new Button { Style = (Style)FindResource("GridButton"),Foreground = new SolidColorBrush(Colors.SteelBlue) };
            Grid.SetRow(b, i);
            Grid.SetColumn(b, j);
            GameGrid.Children.Add(b);
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
            SetBGTransform((Button)sender, Color.FromArgb(0x99, 78, 201, 178), Color.FromArgb(0xFF, 78, 201, 178), 0.1, true);
        }

        private void LineButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform((Button)sender, Color.FromArgb(0xFF, 78, 201, 178), Color.FromArgb(0x99, 78, 201, 178), 0.3, true);
        }

        #endregion

        #region Commands

        private RelayCommand<Button> _lineButtonClick;

        public RelayCommand<Button> LineButtonClick
            => _lineButtonClick ?? (_lineButtonClick = new RelayCommand<Button>(s => AddLine(s, Colors.SteelBlue)));

        public void AddLine(Button s, Color t)
        {
            s.MouseEnter -= LineButtonOnME;
            s.MouseLeave -= LineButtonOnML;
            s.IsEnabled = false;
            var i = Grid.GetRow(s);
            var j = Grid.GetColumn(s);
            int h, w;
            if (i%2 == 1) {
                h = (i - 1)/2 + 1;
                w = (j - 2)/2 + 1;
                if (h < 4) {
                    Gaming.ChangeSquareStatus(h, w, Gaming.Direction.UP);
                } else {
                    Gaming.ChangeSquareStatus(h-1, w, Gaming.Direction.DOWN);
                }
            } else {
                h = (i - 2)/2 + 1;
                w = (j - 1)/2 + 1;
                if (w < 5)
                {
                    Gaming.ChangeSquareStatus(h, w, Gaming.Direction.LEFT);
                }
                else
                {
                    Gaming.ChangeSquareStatus(h , w- 1, Gaming.Direction.RIGHT);
                }
            }
            SetBGTransform(s, ((SolidColorBrush)s.Foreground).Color , t, 0.5, true);
        }

        #endregion
    }
}