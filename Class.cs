using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsLibrary
{
    public abstract class Figure
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Brush FillColor { get; set; }
        public Brush StrokeColor { get; set; }
        public double StrokeThickness { get; set; }

        public Figure(double x, double y, double width, double height, Brush fillColor, Brush strokeColor, double strokeThickness)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            FillColor = fillColor;
            StrokeColor = strokeColor;
            StrokeThickness = strokeThickness;
        }


        public abstract void Draw(Canvas canvas);
    }

    public class RectangleFigure : Figure
    {
        public RectangleFigure(double x, double y, double width, double height, Brush fillColor, Brush strokeColor, double strokeThickness)
            : base(x, y, width, height, fillColor, strokeColor, strokeThickness)
        {
        }
        public override void Draw(Canvas canvas)
        {
            var rectangle = new Rectangle
            {
                Width = Width,
                Height = Height,
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            Canvas.SetLeft(rectangle, X);
            Canvas.SetTop(rectangle, Y);

            canvas.Children.Add(rectangle);
        }
    }

    public class CircleFigure : Figure
    {
        public CircleFigure(double x, double y, double diameter, Brush fillColor, Brush strokeColor, double strokeThickness)
            : base(x - diameter / 2, y - diameter / 2, diameter, diameter, fillColor, strokeColor, strokeThickness)
        {
        }

        public override void Draw(Canvas canvas)
        {
            var ellipse = new Ellipse
            {
                Width = Width,
                Height = Height,
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            Canvas.SetLeft(ellipse, X);
            Canvas.SetTop(ellipse, Y);

            canvas.Children.Add(ellipse);
        }
    }
}