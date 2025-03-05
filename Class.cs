using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace GraphicsLibrary
{
    /// <summary>
    /// Абстркатный класс фигура
    /// </summary>
    public abstract class Figure
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Brush? FillColor { get; set; }
        public Brush StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        public Figure(double x, double y, double width, double height, Brush? fillColor, Brush strokeColor, double strokeThickness)
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

    /// <summary>
    /// Класс, представляющий прямоугольник на канвасе
    /// </summary>
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
                StrokeThickness = StrokeThickness,
                Name = "Rectangle"
            };

            Canvas.SetLeft(rectangle, X);
            Canvas.SetTop(rectangle, Y);

            canvas.Children.Add(rectangle);
        }
    }

    /// <summary>
    /// Класс, представляющий круг на канвасе
    /// </summary>
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
                StrokeThickness = StrokeThickness,
                Name = "Circle"
            };

            Canvas.SetLeft(ellipse, X);
            Canvas.SetTop(ellipse, Y);

            canvas?.Children.Add(ellipse);
        }
    }

    /// <summary>
    /// Класс, представляющий линию на канвасе
    /// </summary>
    public class LineFigure : Figure
    {
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public LineFigure(double x, double y, double x2, double y2, Brush strokeColor, double strokeThickness)
            : base(x, y, x2 - x, y2 - y, null, strokeColor, strokeThickness)
        {
            X2 = x2;
            Y2 = y2;
        }
        public override void Draw(Canvas canvas)
        {
            var line = new Line
            {
                Width = Math.Abs(X2 - X),
                Height = Math.Abs(Y2 - Y),
                X2 = X2 - X,
                Y2 = Y2 - Y,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness,
                Name = "Line"
            };

            Canvas.SetLeft(line, X);
            Canvas.SetTop(line, Y);

            canvas.Children.Add(line);
        }
    }

    /// <summary>
    /// Класс, представляющий треугольник на канвасе
    /// </summary>
    public class TriangleFigure : Figure
    {
        public TriangleFigure(double x, double y, double width, double height, Brush fillColor, Brush strokeColor, double strokeThickness)
            : base(x, y, width, height, fillColor, strokeColor, strokeThickness)
        {
        }

        public override void Draw(Canvas canvas)
        {
            var triangle = new Polygon
            {
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness,
                Width = Width,
                Height = Height,
                Points = new PointCollection
            {
                new Point(1, 1),
                new Point(Width, 1),
                new Point(Width / 2, Height)
            },
                Name = "Triangle"
            };

            Canvas.SetLeft(triangle, X);
            Canvas.SetTop(triangle, Y);

            canvas?.Children.Add(triangle);
        }
    }

    /// <summary>
    /// Класс, представляющий правильный треугольник на канвасе
    /// </summary>
    public class RightTriangleFigure : Figure
    {
        public RightTriangleFigure(double x, double y, double baseLength, double height, Brush fillColor, Brush strokeColor, double strokeThickness)
            : base(x, y, baseLength, height, fillColor, strokeColor, strokeThickness)
        {
        }
        public override void Draw(Canvas canvas)
        {
            var triangle = new Polygon
            {
                Points = new PointCollection
        {
            new Point(1, 1),
            new Point(Width, 1),
            new Point(1, Height)
        },
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness,
                Width = Width,
                Height = Height,
                Name = "RightTriangle"
            };

            Canvas.SetLeft(triangle, X);
            Canvas.SetTop(triangle, Y);

            canvas.Children.Add(triangle);
        }
    }

    /// <summary>
    /// Класс, представляющий ромб на канвасе
    /// </summary>
    public class RhombusFigure : Figure
    {
        public RhombusFigure(double x, double y, double width, double height, Brush fillColor, Brush strokeColor, double strokeThickness)
            : base(x, y, width, height, fillColor, strokeColor, strokeThickness)
        {
        }
        public override void Draw(Canvas canvas)
        {
            var rhombus = new Polygon
            {
                Points = new PointCollection
        {
            new Point(1 + Width / 2, 1),
            new Point(1 + Width, 1 + Height / 2),
            new Point(1 + Width / 2, 1 + Height),
            new Point(1, 1 + Height / 2)
        },
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness,
                Name = "Rhombus",
                Width = Width,
                Height = Height
            };

            Canvas.SetLeft(rhombus, X);
            Canvas.SetTop(rhombus, Y);

            canvas.Children.Add(rhombus);
        }
    }

    /// <summary>
    /// Класс, представляющий звезду на канвасе
    /// </summary>
    public class GoldenStarFigure : Figure
    {
        public GoldenStarFigure(double x, double y, double radius, Brush fillColor, Brush strokeColor, double strokeThickness)
            : base(x, y, radius, radius, fillColor, strokeColor, strokeThickness)
        {
        }

        public override void Draw(Canvas canvas)
        {
            var points = new PointCollection();
            double angle = Math.PI / 5;
            for (int i = 0; i < 10; i++)
            {
                double r = i % 2 == 0 ? Width / 2 : Width / 4; //                                            
                double xOffset = Math.Cos(-Math.PI / 2 + i * angle) * r;
                double yOffset = Math.Sin(-Math.PI / 2 + i * angle) * r;
                points.Add(new Point(xOffset + Width / 2, yOffset + Height / 2));
            }

            var star = new Polygon
            {
                Points = points,
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness,
                Name = "GoldenStar",
                Width = Width,
                Height = Height
            };

            Canvas.SetLeft(star, X);
            Canvas.SetTop(star, Y);

            canvas.Children.Add(star);
        }
    }

    /// <summary>
    /// Класс, представляющий человечка на канвасе
    /// </summary>
    public class PersonFigure : Figure
    {
        public PersonFigure(double x, double y, double width, double height, Brush fillColor, Brush strokeColor, double strokeThickness)
            : base(x, y, width, height, fillColor, strokeColor, strokeThickness)
        {
        }
        public override void Draw(Canvas canvas)
        {
            var head = new CircleFigure(X, Y, Width / 3, FillColor, StrokeColor, StrokeThickness);

            var body = new LineFigure(X + Width / 2, Y + Width / 3, X + Width / 2, Y + Height / 3 * 2, StrokeColor, StrokeThickness);

            var leftArm = new LineFigure(X, Y + Height / 2, X + Width / 2, Y + Height / 3, StrokeColor, StrokeThickness);

            var rightArm = new LineFigure(X + Width, Y + Height / 2, X + Width / 2, Y + Height / 3, StrokeColor, StrokeThickness);

            var leftLeg = new LineFigure(X + Width / 2, Y + Height / 3 * 2, X, Y + Height, StrokeColor, StrokeThickness);

            var rightLeg = new LineFigure(X + Width / 2, Y + Height / 3 * 2, X + Width, Y + Height, StrokeColor, StrokeThickness);

            head.Draw(canvas);
            body.Draw(canvas);
            leftArm.Draw(canvas);
            rightArm.Draw(canvas);
            leftLeg.Draw(canvas);
            rightLeg.Draw(canvas);
        }
    }
}