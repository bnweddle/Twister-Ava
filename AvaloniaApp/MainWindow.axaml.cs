using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace AvaloniaApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void TwisterSpinner_OnClicked(object sender, RoutedEventArgs e)
        {
            if (TwisterSpinner.SlicesNumber > 1)
            {
                TwisterSpinner.GenerateSpinner();
                //((EasingDoubleKeyFrame)Resources["FinishFrame"]).Value = TwisterSpinner.SpinnerAngle;
            }
        }

        private void CircleColor_OnClicked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(OutsideGrid.Height);
            Debug.WriteLine(OutsideGrid.Bounds.Height);

            CircleColor circleColor = (CircleColor)sender;
            if (circleColor.Selected)
            {
                TwisterSpinner.AddColor(circleColor.CircleFill, circleColor.ColorName);
            }
            else
            {
                TwisterSpinner.RemoveColor(circleColor.CircleFill, circleColor.ColorName);
            }
        }


    }
}