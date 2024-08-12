using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AvaloniaApp;

public partial class Slice : UserControl
{
    #region Dependency Properties and Events

    // Register the RoutedEvent
    public static readonly RoutedEvent<RoutedEventArgs> OnClickedEvent =
        RoutedEvent.Register<Slice, RoutedEventArgs>(nameof(OnClicked), RoutingStrategies.Bubble);

    // CLR event wrapper for adding/removing handlers
    public event EventHandler<RoutedEventArgs> OnClicked
    {
        add { AddHandler(OnClickedEvent, value); }
        remove { RemoveHandler(OnClickedEvent, value); }
    }


    // Define the AvaloniaProperty
    public static readonly StyledProperty<double> RadiusProperty =
        AvaloniaProperty.Register<Slice, double>(nameof(Radius));

    // Property wrapper for easier use
    public double Radius
    {
        get => GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    #endregion

    private Random random = new Random();

    public double SlicesNumber { get; set; } = 0;
    private static List<Avalonia.Controls.Shapes.Path> paths = new List<Avalonia.Controls.Shapes.Path>();
    private static List<Label> textBlocks = new List<Label>();
    private static List<IBrush> brushes = new List<IBrush>();
    private static List<string> colorNames = new List<string>();

    public int SpinnerValue { get; set; }
    public int SpinnerAngle { get; set; }

    public Slice()
    {
        InitializeComponent();
        DataContext = this;
    }

    #region Methods

    public void AddColor(IBrush color, string name)
    {
        brushes.Add(color);
        colorNames.Add(name);
        SlicesNumber++;

        if (SlicesNumber == 1)
        {
            Default.Fill = color;
        }
        else
        {
            Default.Opacity = 0;
            Redraw();
        }

    }

    public void RemoveColor(IBrush color, string name)
    {
        brushes.Remove(color);
        colorNames.Remove(name);

        SlicesNumber--;
        if (SlicesNumber <= 0)
        {
            SlicesNumber = 0;
            Default.Opacity = 1;
            Default.Fill = Brushes.Tan;
        }
        else if (SlicesNumber == 1)
        {
            Default.Fill = brushes[0];
            Default.Opacity = 1;
            uxGrid.Children.Clear();
        }
        else
        {
            Default.Opacity = 0;
            Redraw();
        }

    }

    private void Redraw()
    {
        uxGrid.Children.Clear();
        paths.Clear();
        textBlocks.Clear();

        double angle = 360 / SlicesNumber;
        double radius = Radius;

        int i = 0;
        for (double s = 0; s < 360; s += angle)
        {
            AddSlice(brushes[i], s, radius, angle);
            AddSlice(LightenBrush(((IImmutableSolidColorBrush)brushes[i]).Color), s, radius, angle, 5);
            AddLabel(s, angle, radius, colorNames[i]);
            i++;
        }

        for (int p = 0; p < paths.Count(); p++)
        {
            uxGrid.Children.Add(paths[p]);

        }

        for (int t = 0; t < textBlocks.Count(); t++)
        {
            uxGrid.Children.Add(textBlocks[t]);
        }
    }


    private static void AddSlice(IBrush color, double s, double radius, double angle)
    {
        Avalonia.Controls.Shapes.Path p = new Avalonia.Controls.Shapes.Path();

        p.Fill = color;
        PathFigure pf = new PathFigure();
        pf.StartPoint = new Point(radius * Math.Sin(s / 180 * Math.PI), radius * Math.Cos(s / 180 * Math.PI));

        pf.Segments.Add(new ArcSegment
        {
            Point = new Point(radius * Math.Sin((s + angle) / 180 * Math.PI), radius * Math.Cos((s + angle) / 180 * Math.PI)),
            Size = new Size(radius, radius),
            IsLargeArc = false
        });
        pf.Segments.Add(new LineSegment
        {
            Point = new Point(0, 0)
        });

        PathGeometry pg = new PathGeometry();
        pg.Figures.Add(pf);
        p.Data = pg;

        paths.Add(p);
    }

    private static void AddSlice(IBrush color, double s, double radius, double angle, double thickness)
    {
        Point startpoint = new Point((radius - thickness) * Math.Sin(s / 180 * Math.PI), (radius - thickness) * Math.Cos(s / 180 * Math.PI));
        Point endpoint = new Point((radius - thickness) * Math.Sin((s + angle) / 180 * Math.PI), (radius - thickness) * Math.Cos((s + angle) / 180 * Math.PI));
        Vector startunitinvert = new Vector(-startpoint.Y, startpoint.X);
        startunitinvert = Vector.Multiply(startunitinvert, 1 / startunitinvert.Length);

        Vector endunitinvert = new Vector(-endpoint.Y, endpoint.X);
        endunitinvert = Vector.Multiply(endunitinvert, 1 / endunitinvert.Length);
        Avalonia.Controls.Shapes.Path p = new Avalonia.Controls.Shapes.Path();

        p.Fill = color;
        p.Effect = new BlurEffect { Radius = 4 };

        PathFigure pf = new PathFigure();

        startpoint = new Point(startpoint.X - startunitinvert.X * thickness, startpoint.Y - startunitinvert.Y * thickness);
        endpoint = new Point(endpoint.X + endunitinvert.X * thickness, endpoint.Y + endunitinvert.Y * thickness);

        //startpoint.X -= startunitinvert.X * thickness;
        //startpoint.Y -= startunitinvert.Y * thickness;
        
        
        //endpoint.X += endunitinvert.X * thickness;
        //endpoint.Y += endunitinvert.Y * thickness;

        pf.StartPoint = startpoint;

        pf.Segments.Add(new ArcSegment
        {
            Point = endpoint,
            Size = new Size(((radius + .1) - thickness), ((radius + .1) - thickness)),
            IsLargeArc = false
        });
        pf.Segments.Add(new LineSegment
        {
            Point = new Point((thickness) * Math.Sin((s + angle / 2) / 180 * Math.PI), (thickness) * Math.Cos((s + angle / 2) / 180 * Math.PI))
        });

        PathGeometry pg = new PathGeometry();
        pg.Figures.Add(pf);
        p.Data = pg;

        paths.Add(p);
    }



    private static void AddLabel(double s, double angle, double radius, string name)
    {
        Label tb = new Label();
        tb.Content = new OutlinedText()
        {
            StrokeThickness = 2,
            Stroke = Brushes.Black,
            Fill = Brushes.White,
            FontWeight = FontWeight.Bold,
            Text = name
        };

        tb.HorizontalContentAlignment = HorizontalAlignment.Left;
        tb.VerticalContentAlignment = VerticalAlignment.Top;
        tb.HorizontalAlignment = HorizontalAlignment.Left;
        tb.VerticalAlignment = VerticalAlignment.Top;

        tb.Background = Brushes.Transparent;
        tb.FontSize = 15;

        TransformGroup tg = new TransformGroup();
        RotateTransform rt = new RotateTransform(-s + 90 - (angle / 2));
        RotateTransform rts = new RotateTransform(angle / 2);
        TranslateTransform tt = new TranslateTransform(-tb.Height / 2, -tb.Width / 2);
        TranslateTransform tts = new TranslateTransform(radius / 2 - tb.Width / 4, -tb.FontSize);


        tg.Children.Add(tts);
        tg.Children.Add(rt);

        tb.RenderTransform = tg;

        textBlocks.Add(tb);
    }

    public static IBrush LightenBrush(Color source, float correctionFactor = 0.3f)
    {
        var red = (255 - source.R) * correctionFactor + source.R;
        var green = (255 - source.G) * correctionFactor + source.G;
        var blue = (255 - source.B) * correctionFactor + source.B;
        return new SolidColorBrush(Color.FromArgb(source.A, (byte)red, (byte)green, (byte)blue));
    }

    public static Color LightenColor(Color source, float correctionFactor = 0.3f)
    {
        var red = (255 - source.R) * correctionFactor + source.R;
        var green = (255 - source.G) * correctionFactor + source.G;
        var blue = (255 - source.B) * correctionFactor + source.B;
        return Color.FromArgb(source.A, (byte)red, (byte)green, (byte)blue);
    }

    public string CaptureAnswerColor()
    {
        int anglediff;
        double SliceSize = 360 / SlicesNumber;
        anglediff = (SpinnerAngle + 180) % 360;
        Debug.WriteLine("anglediff:" + anglediff);


        for (int i = 0; i < colorNames.Count; i++)
        {
            if (anglediff >= i * SliceSize && anglediff < (i + 1) * SliceSize)
            {
                return colorNames[i];
            }
        }

        return colorNames[SpinnerValue - 1];
    }


    #endregion

    public void GenerateSpinner()
    {
        SpinnerAngle = random.Next(2160, 2520);
    }

    private void Path_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(OnClickedEvent));
    }
}