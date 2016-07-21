using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace LiCalculatorWPF
{
    //https://github.com/Esri/route-planner-csharp/blob/master/RoutePlanner_DeveloperTools/Source/ArcLogisticsApp/GridLengthAnimation.cs

    /// <summary>
    /// Enables animation on gridLength(column and rows)
    /// </summary>
    public class GridLengthAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType => typeof(GridLength);

        protected override Freezable CreateInstanceCore() => new GridLengthAnimation();

        #region From

        /// <summary>
        /// From Dependency Property
        /// </summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation),
                new FrameworkPropertyMetadata(GridLength.Auto));

        /// <summary>
        /// Gets or sets the From property. This dependency property 
        /// indicates the from value of the animation.
        /// </summary>
        public GridLength From
        {
            get { return (GridLength)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        #endregion

        #region To

        /// <summary>
        /// To Dependency Property
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation),
                new FrameworkPropertyMetadata(GridLength.Auto));

        /// <summary>
        /// Gets or sets the To property. This dependency property 
        /// indicates the to value of the animation.
        /// </summary>
        public GridLength To
        {
            get { return (GridLength)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        #endregion



        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock clock)
        {
            //The main job is to convert the value from GridLenght to Double :)

            double fromValue = From.Value;
            double toValue = To.Value;
            if (clock.CurrentProgress == null) {
                return new GridLength(toValue, GridUnitType.Star);
            }
            if (fromValue > toValue)
            {
                return new GridLength((1 - clock.CurrentProgress.Value) *
                    (fromValue - toValue) + toValue, GridUnitType.Star);
            }
            else
            {
                return new GridLength(clock.CurrentProgress.Value *
                    (toValue - fromValue) + fromValue, GridUnitType.Star);
            }
        }
    }
}