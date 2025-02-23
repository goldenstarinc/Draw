
#region v3

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;
using GraphicsLibrary;
using Microsoft.UI;
using System;
using Windows.Foundation;

namespace App3
{
    public sealed partial class MainWindow : Window
    {
        private Tool selectedTool = Tool.Brush;
        private Polyline currentStroke;
        private bool isDrawing = false;
        private Point startPoint;
        private Figure previewFigure;
        private Canvas previewLayer;

        private bool isDragging = false;
        private Point lastPointerPosition;
        private UIElement selectedElement;

        private enum Tool
        {
            Brush,
            Rectangle,
            Circle
        }

        public MainWindow()
        {
            this.InitializeComponent();
            previewLayer = new Canvas();
            DrawingCanvas.Children.Add(previewLayer);
        }

        private void SelectBrush(object sender, RoutedEventArgs e) => selectedTool = Tool.Brush;

        private void SelectRectangle(object sender, RoutedEventArgs e) => selectedTool = Tool.Rectangle;

        private void SelectCircle(object sender, RoutedEventArgs e) => selectedTool = Tool.Circle;

        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();
            previewLayer = new Canvas();
            DrawingCanvas.Children.Add(previewLayer);
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(DrawingCanvas).Position;
            startPoint = point;
            isDrawing = true;

            selectedElement = e.OriginalSource as UIElement; // Проверяем, нажали ли на фигуру
            if (selectedElement != null && selectedElement is Shape)
            {
                isDragging = true;
                lastPointerPosition = point;
                return;
            }

            if (selectedTool == Tool.Brush)
            {
                currentStroke = new Polyline
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2
                };
                currentStroke.Points.Add(point);
                DrawingCanvas.Children.Add(currentStroke);
            }
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isDragging && selectedElement != null)
            {
                var point = e.GetCurrentPoint(DrawingCanvas).Position;
                double dx = point.X - lastPointerPosition.X;
                double dy = point.Y - lastPointerPosition.Y;
                lastPointerPosition = point;

                if (selectedElement is Shape shape)
                {
                    Canvas.SetLeft(shape, Canvas.GetLeft(shape) + dx);
                    Canvas.SetTop(shape, Canvas.GetTop(shape) + dy);
                }
                return;
            }

            if (!isDrawing) return;

            var pointMove = e.GetCurrentPoint(DrawingCanvas).Position;

            if (selectedTool == Tool.Brush && currentStroke != null)
            {
                currentStroke.Points.Add(pointMove);
            }
            else if (selectedTool == Tool.Rectangle || selectedTool == Tool.Circle)
            {
                double x = Math.Min(startPoint.X, pointMove.X);
                double y = Math.Min(startPoint.Y, pointMove.Y);
                double width = Math.Abs(pointMove.X - startPoint.X);
                double height = Math.Abs(pointMove.Y - startPoint.Y);

                if (selectedTool == Tool.Rectangle)
                {
                    previewFigure = new RectangleFigure(x, y, width, height, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(Colors.Black), 2);
                }
                else if (selectedTool == Tool.Circle)
                {
                    previewFigure = new CircleFigure(x + width / 2, y + height / 2, Math.Max(width, height), new SolidColorBrush(Colors.Transparent), new SolidColorBrush(Colors.Black), 2);
                }

                previewLayer.Children.Clear();
                previewFigure?.Draw(previewLayer);
            }
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isDrawing = false;
            isDragging = false;
            selectedElement = null;

            if (previewFigure != null)
            {
                previewFigure.Draw(DrawingCanvas);
                previewFigure = null;
                previewLayer.Children.Clear();
            }
        }

    }
}
#endregion


#region v1

//using Microsoft.UI.Xaml;
//using Microsoft.UI.Xaml.Controls;
//using Microsoft.UI.Xaml.Input;
//using Microsoft.UI.Xaml.Media;
//using Microsoft.UI.Xaml.Shapes;
//using Windows.UI;
//using GraphicsLibrary;
//using Microsoft.UI;
//using System;
//using Windows.Foundation;

//namespace App3
//{
//    public sealed partial class MainWindow : Window
//    {
//        private Tool selectedTool = Tool.Brush;
//        private Polyline currentStroke;
//        private bool isDrawing = false;
//        private Point startPoint;
//        private Figure previewFigure;
//        private Canvas previewLayer;
//        private enum Tool
//        {
//            Brush,
//            Rectangle,
//            Circle
//        }

//        public MainWindow()
//        {
//            this.InitializeComponent();
//            previewLayer = new Canvas();
//            DrawingCanvas.Children.Add(previewLayer);
//        }

//        private void SelectBrush(object sender, RoutedEventArgs e) => selectedTool = Tool.Brush;

//        private void SelectRectangle(object sender, RoutedEventArgs e) => selectedTool = Tool.Rectangle;

//        private void SelectCircle(object sender, RoutedEventArgs e) => selectedTool = Tool.Circle;

//        private void ClearCanvas(object sender, RoutedEventArgs e)
//        {
//            DrawingCanvas.Children.Clear();
//            previewLayer = new Canvas();
//            DrawingCanvas.Children.Add(previewLayer);
//        }

//        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
//        {
//            var point = e.GetCurrentPoint(DrawingCanvas).Position;
//            startPoint = point;
//            isDrawing = true;

//            if (selectedTool == Tool.Brush)
//            {
//                currentStroke = new Polyline
//                {
//                    Stroke = new SolidColorBrush(Colors.Black),
//                    StrokeThickness = 2
//                };
//                currentStroke.Points.Add(point);
//                DrawingCanvas.Children.Add(currentStroke);
//            }
//        }

//        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
//        {
//            if (!isDrawing) return;

//            var point = e.GetCurrentPoint(DrawingCanvas).Position;

//            if (selectedTool == Tool.Brush && currentStroke != null)
//            {
//                currentStroke.Points.Add(point);
//            }
//            else if (selectedTool == Tool.Rectangle || selectedTool == Tool.Circle)
//            {
//                double x = Math.Min(startPoint.X, point.X);
//                double y = Math.Min(startPoint.Y, point.Y);
//                double width = Math.Abs(point.X - startPoint.X);
//                double height = Math.Abs(point.Y - startPoint.Y);

//                if (selectedTool == Tool.Rectangle)
//                {
//                    previewFigure = new RectangleFigure(x, y, width, height, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(Colors.Black), 2);
//                }
//                else if (selectedTool == Tool.Circle)
//                {
//                    previewFigure = new CircleFigure(x + width / 2, y + height / 2, Math.Max(width, height), new SolidColorBrush(Colors.Transparent), new SolidColorBrush(Colors.Black), 2);
//                }

//                previewLayer.Children.Clear();
//                previewFigure?.Draw(previewLayer);
//            }
//        }

//        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
//        {
//            if (!isDrawing) return;
//            isDrawing = false;

//            if (previewFigure != null)
//            {
//                previewFigure.Draw(DrawingCanvas);
//                previewFigure = null;
//                previewLayer.Children.Clear();
//            }

//            int c = DrawingCanvas.Children.Count;
//        }
//    }
//}

#endregion

#region v2

//using Microsoft.UI.Xaml;
//using Microsoft.UI.Xaml.Controls;
//using Microsoft.UI.Xaml.Input;
//using Microsoft.UI.Xaml.Media;
//using Microsoft.UI.Xaml.Shapes;
//using Windows.UI;
//using GraphicsLibrary;
//using System;
//using Windows.Foundation;
//using Microsoft.UI;

//namespace App3
//{
//    public sealed partial class MainWindow : Window
//    {
//        private enum Tool { Brush, Rectangle, Circle }
//        private Tool selectedTool = Tool.Brush;
//        private Polyline currentStroke;
//        private bool isDrawing = false;
//        private Point startPoint;
//        private Figure previewFigure;
//        private Canvas previewLayer;
//        private Figure selectedFigure;
//        private Rectangle selectionRectangle;
//        private Canvas resizeHandles;

//        public MainWindow()
//        {
//            this.InitializeComponent();
//            previewLayer = new Canvas();
//            DrawingCanvas.Children.Add(previewLayer);
//        }

//        private void SelectBrush(object sender, RoutedEventArgs e)
//        {
//            selectedTool = Tool.Brush;
//        }

//        private void SelectRectangle(object sender, RoutedEventArgs e)
//        {
//            selectedTool = Tool.Rectangle;
//        }

//        private void SelectCircle(object sender, RoutedEventArgs e)
//        {
//            selectedTool = Tool.Circle;
//        }

//        private void ClearCanvas(object sender, RoutedEventArgs e)
//        {
//            DrawingCanvas.Children.Clear();
//            previewLayer = new Canvas();
//            DrawingCanvas.Children.Add(previewLayer);
//            selectedFigure = null;
//            RemoveSelection();
//        }

//        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
//        {
//            var point = e.GetCurrentPoint(DrawingCanvas).Position;
//            startPoint = point;
//            isDrawing = true;

//            if (selectedTool == Tool.Brush)
//            {
//                currentStroke = new Polyline
//                {
//                    Stroke = new SolidColorBrush(Colors.Black),
//                    StrokeThickness = 2
//                };
//                currentStroke.Points.Add(point);
//                DrawingCanvas.Children.Add(currentStroke);
//            }
//            else
//            {
//                CheckFigureSelection(point);
//            }
//        }

//        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
//        {
//            if (!isDrawing) return;

//            var point = e.GetCurrentPoint(DrawingCanvas).Position;

//            if (selectedTool == Tool.Brush && currentStroke != null)
//            {
//                currentStroke.Points.Add(point);
//            }
//            else if (selectedTool == Tool.Rectangle || selectedTool == Tool.Circle)
//            {
//                double x = Math.Min(startPoint.X, point.X);
//                double y = Math.Min(startPoint.Y, point.Y);
//                double width = Math.Abs(point.X - startPoint.X);
//                double height = Math.Abs(point.Y - startPoint.Y);

//                if (selectedTool == Tool.Rectangle)
//                {
//                    previewFigure = new RectangleFigure(x, y, width, height, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(Colors.Black), 2);
//                }
//                else if (selectedTool == Tool.Circle)
//                {
//                    previewFigure = new CircleFigure(x + width / 2, y + height / 2, Math.Max(width, height), new SolidColorBrush(Colors.Transparent), new SolidColorBrush(Colors.Black), 2);
//                }

//                previewLayer.Children.Clear();
//                previewFigure?.Draw(previewLayer);
//            }
//        }

//        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
//        {
//            if (!isDrawing) return;
//            isDrawing = false;

//            if (previewFigure != null)
//            {
//                previewFigure.Draw(DrawingCanvas);
//                previewFigure = null;
//                previewLayer.Children.Clear();
//            }
//        }

//        private void CheckFigureSelection(Point point)
//        {
//            foreach (var child in DrawingCanvas.Children)
//            {
//                if (child is Shape shape)
//                {
//                    double left = Canvas.GetLeft(shape);
//                    double top = Canvas.GetTop(shape);
//                    double right = left + shape.Width;
//                    double bottom = top + shape.Height;

//                    if (point.X >= left && point.X <= right && point.Y >= top && point.Y <= bottom)
//                    {
//                        SelectFigure(shape, left, top);
//                        return;
//                    }
//                }
//            }

//            RemoveSelection();
//        }

//        private void SelectFigure(Shape shape, double x, double y)
//        {
//            selectedFigure = new RectangleFigure(x, y, shape.Width, shape.Height, shape.Fill, shape.Stroke, shape.StrokeThickness);
//            DrawSelectionRectangle(x, y, shape.Width, shape.Height);
//        }

//        private void DrawSelectionRectangle(double x, double y, double width, double height)
//        {
//            RemoveSelection(); // Удаляем старое выделение

//            selectionRectangle = new Rectangle
//            {
//                Width = width + 10,
//                Height = height + 10,
//                Stroke = new SolidColorBrush(Colors.Blue),
//                StrokeThickness = 2,
//                StrokeDashArray = new DoubleCollection() { 4, 4 }
//            };

//            Canvas.SetLeft(selectionRectangle, x - 5);
//            Canvas.SetTop(selectionRectangle, y - 5);
//            DrawingCanvas.Children.Add(selectionRectangle);

//            // Обновляем позиции квадратиков
//            DrawResizeHandles(x, y, width, height);
//        }


//        private void DrawResizeHandles(double x, double y, double width, double height)
//        {
//            if (resizeHandles != null)
//                DrawingCanvas.Children.Remove(resizeHandles);

//            resizeHandles = new Canvas();

//            double[] handleOffsets = { -5, width - 5 };
//            foreach (double dx in handleOffsets)
//            {
//                foreach (double dy in handleOffsets)
//                {
//                    var handle = new Rectangle
//                    {
//                        Width = 10,
//                        Height = 10,
//                        Fill = new SolidColorBrush(Colors.White),
//                        Stroke = new SolidColorBrush(Colors.Black),
//                        StrokeThickness = 1
//                    };

//                    Canvas.SetLeft(handle, x + dx);
//                    Canvas.SetTop(handle, y + dy);

//                    handle.PointerPressed += ResizeHandle_PointerPressed;
//                    resizeHandles.Children.Add(handle);
//                }
//            }

//            DrawingCanvas.Children.Add(resizeHandles);
//        }


//        private void ResizeHandle_PointerPressed(object sender, PointerRoutedEventArgs e)
//        {
//            if (sender is Rectangle handle)
//            {
//                handle.CapturePointer(e.Pointer);
//                handle.PointerMoved += ResizeHandle_PointerMoved;
//                handle.PointerReleased += ResizeHandle_PointerReleased;
//            }
//        }

//        private void ResizeHandle_PointerMoved(object sender, PointerRoutedEventArgs e)
//        {
//            if (selectedFigure == null || sender is not Rectangle handle) return;

//            var point = e.GetCurrentPoint(DrawingCanvas).Position;
//            double x = Canvas.GetLeft(selectionRectangle) + 5;
//            double y = Canvas.GetTop(selectionRectangle) + 5;
//            double newWidth = point.X - x;
//            double newHeight = point.Y - y;

//            // Минимальные размеры фигуры
//            newWidth = Math.Max(newWidth, 10);
//            newHeight = Math.Max(newHeight, 10);

//            // Удаляем старую фигуру перед отрисовкой новой
//            if (selectedFigure.Shape != null)
//                DrawingCanvas.Children.Remove(selectedFigure.Shape);

//            selectedFigure.Width = newWidth;
//            selectedFigure.Height = newHeight;

//            // Перерисовываем фигуру
//            selectedFigure.Draw(DrawingCanvas);

//            // Перерисовываем рамку выделения
//            DrawSelectionRectangle(x, y, selectedFigure.Width, selectedFigure.Height);
//        }


//        private void ResizeHandle_PointerReleased(object sender, PointerRoutedEventArgs e)
//        {
//            if (sender is Rectangle handle)
//            {
//                handle.ReleasePointerCapture(e.Pointer);
//                handle.PointerMoved -= ResizeHandle_PointerMoved;
//                handle.PointerReleased -= ResizeHandle_PointerReleased;
//            }
//        }

//        private void RemoveSelection()
//        {
//            DrawingCanvas.Children.Remove(selectionRectangle);
//            DrawingCanvas.Children.Remove(resizeHandles);
//            selectionRectangle = null;
//            resizeHandles = null;
//        }
//    }
//}

#endregion