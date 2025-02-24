#region PACKAGES
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
using Windows.Devices.PointOfService.Provider;
using System.Diagnostics;
#endregion

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

        private Color defualtCanvasColor = Colors.White;

        private bool isDragging = false;
        private Point lastPointerPosition;
        private UIElement selectedElement;
        private Rectangle selectionRectangle;

        private bool isResizing = false;
        private string resizeDirection = "";
        private enum Tool
        {
            Brush,
            Fill,
            Rectangle,
            Circle
        }

        public MainWindow()
        {
            this.InitializeComponent();
            previewLayer = new Canvas();
            DrawingCanvas.Children.Add(previewLayer);
            DrawingCanvas.Background = new SolidColorBrush(Colors.White);
        }

        /// <summary>
        /// Метод, позволяющий выбрать кисть
        /// </summary>
        private void SelectBrush(object sender, RoutedEventArgs e) => selectedTool = Tool.Brush;

        /// <summary>
        /// Метод, позволяющий выбрать прямоугольник
        /// </summary>
        private void SelectRectangle(object sender, RoutedEventArgs e) => selectedTool = Tool.Rectangle;

        /// <summary>
        /// Метод, позволяющий выбрать круг
        /// </summary>
        private void SelectCircle(object sender, RoutedEventArgs e) => selectedTool = Tool.Circle;

        /// <summary>
        /// Метод, позволяющий выбрать заливку
        /// </summary>
        private void SelectFill(object sender, RoutedEventArgs e) => selectedTool = Tool.Fill;

        /// <summary>
        /// Метод для очищения канваса
        /// </summary>
        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();
            previewLayer = new Canvas();
            DrawingCanvas.Children.Add(previewLayer);
            DrawingCanvas.Background = new SolidColorBrush(Colors.White);
        }

        /// <summary>
        /// Метод, отслеживающий одиночное нажатие курсора
        /// </summary>
        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(DrawingCanvas).Position;
            startPoint = point;
            isDrawing = true;

            if (selectionRectangle != null)
            {
                if (IsOnBorder(point, selectionRectangle, out resizeDirection))
                {
                    MakeResizingActive();
                    return;
                }
                else if (selectedElement != null && selectedElement is Shape && IsPointInsideElement(point, selectedElement))
                {
                    MakeDraggingActive();
                }
                else
                {
                    RemoveSelection();
                    MakeDraggingInactive();
                }
                isDrawing = false;
                lastPointerPosition = point;
                return;
            }

            selectedElement = e.OriginalSource as UIElement;

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
            else if (selectedTool == Tool.Fill)
            {
                if (IsPointInsideElement(point, selectedElement))
                {
                    Shape selectedShape = selectedElement as Shape;

                    if (selectedShape != null)
                    {
                        selectedShape.Fill = new SolidColorBrush(Colors.Green);
                    }
                }
                else
                {
                    DrawingCanvas.Background = new SolidColorBrush(Colors.Green);
                }
            }
        }

        /// <summary>
        /// Метод, отслеживающий одиночное движение курсора
        /// </summary>
        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isResizing && !string.IsNullOrEmpty(resizeDirection))
            {
                var point = e.GetCurrentPoint(DrawingCanvas).Position;
                double dx = point.X - startPoint.X;
                double dy = point.Y - startPoint.Y;

                Shape shape = selectedElement as Shape;

                if (shape != null)
                {
                    if (resizeDirection.Contains("right"))
                    {
                        if (shape.Width + dx > 0)
                        {
                            shape.Width += dx;
                        }
                    }
                    if (resizeDirection.Contains("left"))
                    {
                        if (shape.Width - dx > 0)
                        {
                            shape.Width -= dx;
                            Canvas.SetLeft(shape, Canvas.GetLeft(shape) + dx);
                        }
                    }
                    if (resizeDirection.Contains("bottom"))
                    {
                        if (shape.Height + dy > 0)
                        {
                            shape.Height += dy;
                        }
                    }
                    if (resizeDirection.Contains("top"))
                    {
                        if (shape.Height - dy > 0)
                        {
                            shape.Height -= dy;
                            Canvas.SetTop(shape, Canvas.GetTop(shape) + dy);
                        }
                    }
                    RemoveSelectionRectangle(ref selectionRectangle);
                }

                startPoint = point;
                return;
            }
            
            if (isDragging && selectedElement != null)
            {
                if (selectionRectangle != null)
                {
                    RemoveSelectionRectangle(ref selectionRectangle);
                }
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
                    previewFigure = new RectangleFigure(x, y, width, height, new SolidColorBrush(defualtCanvasColor), new SolidColorBrush(Colors.Black), 2);
                }
                else if (selectedTool == Tool.Circle)
                {
                    previewFigure = new CircleFigure(x + width / 2, y + height / 2, Math.Max(width, height), new SolidColorBrush(defualtCanvasColor), new SolidColorBrush(Colors.Black), 2);
                }

                previewLayer.Children.Clear();
                previewFigure?.Draw(previewLayer);
            }
        }
     
        /// <summary>
        /// Метод, отслеживающий отжатие курсора
        /// </summary>
        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if ((isDragging || isResizing) && selectedElement != null && selectedElement is Shape)
            {
                DrawSelectionRectangle(selectedElement);
            }
            isDrawing = false;
            MakeDraggingInactive();
            MakeResizingInactive();
            if (previewFigure != null)
            {
                previewFigure.Draw(DrawingCanvas);
                previewFigure = null;
                previewLayer.Children.Clear();
            }
        }

        /// <summary>
        /// Метод, выделяющий выбранную фигуру
        /// </summary>
        /// <param name="element"></param>
        private void DrawSelectionRectangle(UIElement element)
        {
            RemoveSelectionRectangle(ref selectionRectangle);

            if (element is Shape shape)
            {
                double left = Canvas.GetLeft(shape);
                double top = Canvas.GetTop(shape);
                double width = shape.Width;
                double height = shape.Height;

                selectionRectangle = new Rectangle
                {
                    Width = width + 8,
                    Height = height + 8, 
                    Stroke = new SolidColorBrush(Colors.Black), 
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(Color.FromArgb(50, 57, 97, 255))
                };

                Canvas.SetLeft(selectionRectangle, left - 4);
                Canvas.SetTop(selectionRectangle, top - 4);

                DrawingCanvas.Children.Add(selectionRectangle);
            }
        }

        /// <summary>
        /// Метод, снимающий выделение с фигуры
        /// </summary>
        private void RemoveSelection()
        {
            if (selectionRectangle != null)
            {
                RemoveSelectionRectangle(ref selectionRectangle);
            }
            SetSelectedElementToNull();
        }

        /// <summary>
        /// Метод, отслеживающий двойное нажатие
        /// </summary>
        private void Canvas_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            selectedElement = e.OriginalSource as UIElement; // Проверяем, нажали ли на фигуру
            if (selectedElement != null && selectedElement is Shape)
            {
                DrawSelectionRectangle(selectedElement);
                MakeDraggingActive();
            }
        }

        /// <summary>
        /// Метод, проверяющий содержит ли выбранная фигура переданную точку
        /// </summary>
        /// <param name="point">Переданная точка</param>
        /// <param name="element">Переданный элемент</param>
        /// <returns>True - если выбранная фигура содержит переданную точку, иначе - false</returns>
        private bool IsPointInsideElement(Point point, UIElement element)
        {
            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);
            double width = (element as FrameworkElement)?.Width ?? 0;
            double height = (element as FrameworkElement)?.Height ?? 0;

            return (point.X >= left && point.X <= left + width &&
                    point.Y >= top && point.Y <= top + height);
        }

        /// <summary>
        /// Метод, отслеживающий нажатие клавиши
        /// </summary>
        private void Canvas_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (selectionRectangle != null && e.Key == Windows.System.VirtualKey.Delete)
            {
                RemoveSelectedElement();
                RemoveSelection();
                MakeDraggingInactive();
            }
        }

        /// <summary>
        /// Метод, удаляющий выбранную фигуру
        /// </summary>
        private void RemoveSelectedElement()
        {
            if (selectedElement != null)
            {
                DrawingCanvas.Children.Remove(selectedElement);
            }
        }

        /// <summary>
        /// Метод, выполняющий сброс выбранной фигуры
        /// </summary>
        private void SetSelectedElementToNull() => selectedElement = null;

        /// <summary>
        /// Метод, удаляющий прямоугольник, отвечающий за выделение фигуры
        /// </summary>
        /// <param name="selectionRectangle">Выделяющий прямоугольник</param>
        private void RemoveSelectionRectangle(ref Rectangle selectionRectangle)
        {
            DrawingCanvas.Children.Remove(selectionRectangle);
            selectionRectangle = null;
        }

        /// <summary>
        /// Метод, делающий изменение размера фигуры неактивным
        /// </summary>
        private void MakeResizingInactive() => isResizing = false;

        /// <summary>
        /// Метод, делающий изменение размера фигуры активным
        /// </summary>
        private void MakeResizingActive() => isResizing = true;

        /// <summary>
        /// Метод, делающий перемещение фигуры неактивным
        /// </summary>
        private void MakeDraggingInactive() => isDragging = false;

        /// <summary>
        /// Метод, делающий перемещение фигуры активным
        /// </summary>
        private void MakeDraggingActive() => isDragging = true;

        /// <summary>
        /// Метод, проверяющий находится ли переданная точка на границе выделенной фигуры
        /// </summary>
        /// <param name="point">Переданная точка</param>
        /// <param name="shape">Выделенная фигура</param>
        /// <param name="direction">Направления изменения размера</param>
        /// <returns>True - если точка находится на границе выделенной фигуры, иначе - false</returns>
        private bool IsOnBorder(Point point, Shape shape, out string direction)
        {
            direction = "";
            double left = Canvas.GetLeft(shape);
            double top = Canvas.GetTop(shape);
            double right = left + shape.Width;
            double bottom = top + shape.Height;

            if (Math.Abs(point.X - left) < 5) direction += "left";
            if (Math.Abs(point.X - right) < 5) direction += "right";
            if (Math.Abs(point.Y - top) < 5) direction += "top";
            if (Math.Abs(point.Y - bottom) < 5) direction += "bottom";

            return !string.IsNullOrEmpty(direction);
        }
    }
}

