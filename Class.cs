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
    /// Класс, представляющий линию на канвасе
    /// </summary>
    public class LineFigure : Figure
    {
        public double X2 { get; set; } // Конечная координата X
        public double Y2 { get; set; } // Конечная координата Y

        /// <summary>
        /// Конструктор линии.
        /// </summary>
        public LineFigure(double x, double y, double x2, double y2, Brush strokeColor, double strokeThickness)
            : base(x, y, 0, 0, null, strokeColor, strokeThickness)
        {
            X2 = x2;
            Y2 = y2;
        }

        /// <summary>
        /// Отрисовывает линию на канвасе.
        /// </summary>
        public override void Draw(Canvas canvas)
        {
            var line = new Line
            {
                X1 = X,
                Y1 = Y,
                X2 = X2,
                Y2 = Y2,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            canvas.Children.Add(line);
        }
    }

    /// <summary>
    /// Класс, представляющий прямоугольник
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
                StrokeThickness = StrokeThickness
            };

            Canvas.SetLeft(rectangle, X);
            Canvas.SetTop(rectangle, Y);

            canvas.Children.Add(rectangle);
        }
    }

    /// <summary>
    /// Класс, представляющий круг
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
                StrokeThickness = StrokeThickness
            };

            Canvas.SetLeft(ellipse, X);
            Canvas.SetTop(ellipse, Y);

            canvas?.Children.Add(ellipse);
        }
    }

    /// <summary>
    /// Класс, представляющий треугольник
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
            new Point(X, Y),
            new Point(X + Width, Y),
            new Point(X + Width / 2, Y + Height)
        }
            };

            Canvas.SetLeft(triangle, X);
            Canvas.SetTop(triangle, Y);

            canvas.Children.Add(triangle);
        }
    }

    /// <summary>
    /// Класс, представляющий прямоугольный треугольник
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
             new Point(X, Y),
             new Point(X + Width, Y),
             new Point(X, Y + Height)
         },
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            canvas.Children.Add(triangle);
        }
    }

    /// <summary>
    /// Класс, представляющий ромб
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
             new Point(X + Width / 2, Y),
             new Point(X + Width, Y + Height / 2),
             new Point(X + Width / 2, Y + Height),
             new Point(X, Y + Height / 2)
         },
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            canvas.Children.Add(rhombus);
        }
    }

    /// <summary>
    /// Класс, представляющий звезду
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
                points.Add(new Point(X + xOffset, Y + yOffset));
            }

            var star = new Polygon
            {
                Points = points,
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            canvas.Children.Add(star);
        }
    }

    /// <summary>
    /// Класс, представляющий человечка
    /// </summary>
    public class PersonFigure : Figure
    {
        public PersonFigure(double x, double y, double width, double height, Brush fillColor, Brush strokeColor, double strokeThickness)
            : base(x, y, width, height, fillColor, strokeColor, strokeThickness)
        {
        }

        public override void Draw(Canvas canvas)
        {
            // Голова
            var head = new Ellipse
            {
                Width = Width / 3,
                Height = Width / 3,
                Fill = FillColor,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            Canvas.SetLeft(head, X + Width / 3);
            Canvas.SetTop(head, Y);

            // Тело
            var body = new Line
            {
                X1 = X + Width / 2,
                Y1 = Y + Width / 3,
                X2 = X + Width / 2,
                Y2 = Y + Height / 3 * 2,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            // Руки
            var leftArm = new Line
            {
                X1 = X,
                Y1 = Y + Height / 2,
                X2 = X + Width / 2,
                Y2 = Y + Height / 3,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            var rightArm = new Line
            {
                X1 = X + Width,
                Y1 = Y + Height / 2,
                X2 = X + Width / 2,
                Y2 = Y + Height / 3,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            // Ноги
            var leftLeg = new Line
            {
                X1 = X + Width / 2,
                Y1 = Y + Height / 3 * 2,
                X2 = X,
                Y2 = Y + Height,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            var rightLeg = new Line
            {
                X1 = X + Width / 2,
                Y1 = Y + Height / 3 * 2,
                X2 = X + Width,
                Y2 = Y + Height,
                Stroke = StrokeColor,
                StrokeThickness = StrokeThickness
            };

            canvas.Children.Add(head);
            canvas.Children.Add(body);
            canvas.Children.Add(leftArm);
            canvas.Children.Add(rightArm);
            canvas.Children.Add(leftLeg);
            canvas.Children.Add(rightLeg);
        }
    }


}