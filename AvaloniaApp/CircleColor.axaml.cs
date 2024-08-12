using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;

namespace AvaloniaApp;

public partial class CircleColor : UserControl
{
    #region Styled Properties

    // Register the RoutedEvent
    public static readonly RoutedEvent<RoutedEventArgs> OnClickedEvent =
        RoutedEvent.Register<CircleColor, RoutedEventArgs>(nameof(OnClicked), RoutingStrategies.Bubble);

    // CLR event wrapper for adding/removing handlers
    public event EventHandler<RoutedEventArgs> OnClicked
    {
        add { AddHandler(OnClickedEvent, value); }
        remove { RemoveHandler(OnClickedEvent, value); }
    }


    public bool Selected
    {
        get => GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }

    public static readonly StyledProperty<bool> SelectedProperty =
        AvaloniaProperty.Register<CircleColor, bool>(nameof(Selected), false);

    public string ColorName
    {
        get => GetValue(ColorNameProperty);
        set => SetValue(ColorNameProperty, value);
    }

    public static readonly StyledProperty<string> ColorNameProperty =
        AvaloniaProperty.Register<CircleColor, string>(nameof(ColorName), string.Empty);

    public IBrush CircleBackground
    {
        get => GetValue(CircleBackgroundProperty);
        set => SetValue(CircleBackgroundProperty, value);
    }

    public static readonly StyledProperty<IBrush> CircleBackgroundProperty =
        AvaloniaProperty.Register<CircleColor, IBrush>(nameof(CircleBackground), Brushes.Black);

    public IBrush CircleFill
    {
        get => GetValue(CircleFillProperty);
        set => SetValue(CircleFillProperty, value);
    }

    public static readonly StyledProperty<IBrush> CircleFillProperty =
        AvaloniaProperty.Register<CircleColor, IBrush>(nameof(CircleFill), Brushes.LightGreen);

    public IBrush CircleStroke
    {
        get => GetValue(CircleStrokeProperty);
        set => SetValue(CircleStrokeProperty, value);
    }

    public static readonly StyledProperty<IBrush> CircleStrokeProperty =
        AvaloniaProperty.Register<CircleColor, IBrush>(nameof(CircleStroke), Brushes.Lime);

    #endregion

    public CircleColor()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    private void Ellipse_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        Selected = !Selected;
        RaiseEvent(new RoutedEventArgs(OnClickedEvent, this));
        if (Selected)
        {
            MiddleBorder.Fill = Brushes.White;
        }
        else
        {
            MiddleBorder.Fill = CircleBackground;
        }
    }
}