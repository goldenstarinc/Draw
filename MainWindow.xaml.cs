using GraphicsLibrary;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using static App3.MainWindow;

namespace App3
{
    public sealed partial class MainWindow : Window
    {
        private List<UIElement> _drawnElements = new List<UIElement>();
        private bool _isDrawing = false;
        private UIElement _selectedElement = null; // Selected figure
        private Figure _currentFigure;
        private Point _startPoint;
        private SolidColorBrush _brushColor = new SolidColorBrush(Colors.Black);
        private double _strokeThickness = 2;
        private Point _dragStartPoint;
        private ResizeDirection _resizeDirection;
        private string _currentTool;
        private Point _lastPoint;
        private Path _currentPath;
        private List<Line> _linesList = new List<Line>();
        private int _deleteIndex = -1;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void SelectBrush(object sender, RoutedEventArgs e)
        {
            // Set the current tool to brush
            _currentTool = "Brush";
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            // Toggle theme logic can be implemented here
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            // Logic for creating a new file
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            // Logic for opening a file
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            // Logic for saving a file
        }

        // Clear the canvas
        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();
            _drawnElements.Clear();
            _selectedElement = null;
        }

        // Select Rectangle tool
        private void SelectRectangle(object sender, RoutedEventArgs e)
        {
            _currentTool = "Rectangle";
            _currentFigure = null; // Reset current figure
        }

        // Select Circle tool
        private void SelectCircle(object sender, RoutedEventArgs e)
        {
            _currentTool = "Circle";
            _currentFigure = null; // Reset current figure
        }

        // Handle pointer press (start drawing or selecting)
        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(DrawingCanvas).Position;

            if (_isDrawing)
            {
                return; // If already drawing, ignore
            }

            // Check if there's a figure under the cursor
            _selectedElement = FindElementAtPoint(point);

            if (_selectedElement != null)
            {
                // If clicked on a selected figure, start drag-and-drop
                _dragStartPoint = point;
                _resizeDirection = DetermineResizeDirection(_selectedElement, point);
            }
            else
            {
                // If nothing is selected, start drawing
                _isDrawing = true;
                _startPoint = point;

                switch (_currentTool)
                {
                    case "Rectangle":
                        _currentFigure = new RectangleFigure(
                            _startPoint.X,
                            _startPoint.Y,
                            0,
                            0,
                            null,
                            _brushColor,
                            _strokeThickness
                        );
                        break;
                    case "Circle":
                        _currentFigure = new CircleFigure(
                            _startPoint.X,
                            _startPoint.Y,
                            0,
                            null,
                            _brushColor,
                            _strokeThickness
                        );
                        break;
                    case "Brush":
                        _lastPoint = e.GetCurrentPoint(DrawingCanvas).Position;
                        break;
                    default:
                        _currentFigure = null;
                        break;
                }

                if (_currentFigure != null)
                {
                    _currentFigure.Draw(DrawingCanvas);
                    _drawnElements.Add(DrawingCanvas.Children.Last());
                }
            }
        }

        // Find element at a given point
        private UIElement FindElementAtPoint(Point point)
        {
            foreach (var element in DrawingCanvas.Children)
            {
                if(element is Path path)
                {
                    path.Data.Bounds.Contains(point);

                    if (path.Data.Bounds.Contains(point))
                    {
                        return path;
                    }
                }
                else if (element is Shape shape)
                {
                    Rect bounds = new Rect(
                        Canvas.GetLeft(shape),
                        Canvas.GetTop(shape),
                        shape.Width,
                        shape.Height
                    );

                    if (bounds.Contains(point))
                    {
                        return shape;
                    }
                }
                
            }
            return null;
        }

        // Handle pointer move (draw, resize, or move)
        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var currentPoint = e.GetCurrentPoint(DrawingCanvas).Position;

            if (_isDrawing)
            {
                if (_currentFigure != null)
                {
                    // Draw a new figure dynamically
                    double width = Math.Abs(currentPoint.X - _startPoint.X);
                    double height = Math.Abs(currentPoint.Y - _startPoint.Y);

                    if (_currentFigure is RectangleFigure rectangleFigure)
                    {
                        rectangleFigure.X = Math.Min(_startPoint.X, currentPoint.X);
                        rectangleFigure.Y = Math.Min(_startPoint.Y, currentPoint.Y);
                        rectangleFigure.Width = width;
                        rectangleFigure.Height = height;
                    }
                    else if (_currentFigure is CircleFigure circleFigure)
                    {
                        double diameter = Math.Max(width, height);
                        circleFigure.X = _startPoint.X - diameter / 2;
                        circleFigure.Y = _startPoint.Y - diameter / 2;
                        circleFigure.Width = diameter;
                        circleFigure.Height = diameter;
                    }

                    // Redraw the current figure
                    UpdateFigureOnCanvas();
                }
                else if (_currentTool == "Brush")
                {
                    currentPoint = e.GetCurrentPoint(DrawingCanvas).Position;

                    var line = new Line
                    {
                        X1 = _lastPoint.X,
                        Y1 = _lastPoint.Y,
                        X2 = currentPoint.X,
                        Y2 = currentPoint.Y,
                        Stroke = _brushColor,
                        StrokeThickness = 2
                    };
                    DrawingCanvas.Children.Add(line);
                    _linesList.Add(line);

                    if(_deleteIndex == -1)
                        _deleteIndex = DrawingCanvas.Children.Count - 1;

                    _lastPoint = currentPoint;
                }
            }
            else if (_selectedElement != null)
            {
                // Move or resize the selected figure
                if (_resizeDirection != ResizeDirection.None)
                {
                    ResizeElement(_selectedElement, currentPoint);
                }
                else
                {
                    MoveElement(_selectedElement, currentPoint);
                }
            }
        }

        // Update the figure on the canvas
        private void UpdateFigureOnCanvas()
        {
            if (_currentFigure != null)
            {
                // Remove the previous figure
                if (_drawnElements.Count > 0 && _drawnElements[^1] is UIElement lastElement)
                {
                    DrawingCanvas.Children.Remove(lastElement);
                    _drawnElements.RemoveAt(_drawnElements.Count - 1);
                }

                // Draw the updated figure
                _currentFigure.Draw(DrawingCanvas);
                _drawnElements.Add(DrawingCanvas.Children.Last());
            }
        }

        // Handle pointer release (finalize drawing or selection)
        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isDrawing)
            {
                // Finalize drawing of the new figure
                _isDrawing = false;
                if (_currentFigure != null)
                {
                    _drawnElements.Add(DrawingCanvas.Children.Last());
                }
                if (_currentTool == "Brush")
                {
                    _currentFigure = null;
                    if (_linesList.Count == 0) return;

                    PathFigure pathFigure = new PathFigure();

                    // Add segments to the PathFigure based on the list of lines
                    for (int i = 0; i < _linesList.Count; i++)
                    {
                        if (i == 0)
                        {
                            // Set the start point of the PathFigure to the first line's start point
                            pathFigure.StartPoint = new Point(_linesList[i].X1, _linesList[i].Y1);
                        }

                        // Create a LineSegment and set its Point property
                        LineSegment lineSegment = new LineSegment();
                        lineSegment.Point = new Point(_linesList[i].X2, _linesList[i].Y2);

                        // Add the LineSegment to the PathFigure
                        pathFigure.Segments.Add(lineSegment);

                    }
                    _linesList.Clear();

                    while(DrawingCanvas.Children.Count > _deleteIndex)
                    {
                        DrawingCanvas.Children.RemoveAt(_deleteIndex);
                    }

                    _deleteIndex = -1;

                    Path path = new Path
                    {
                        Stroke = _brushColor,
                        StrokeThickness = 2,
                        Data = new PathGeometry() // Initialize with empty geometry
                    };
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures.Add(pathFigure);

                    path.Data = pathGeometry;
                    DrawingCanvas.Children.Add(path);
                }
            }
            else if (_selectedElement != null)
            {
                // Finalize drag-and-drop or resizing
                _resizeDirection = ResizeDirection.None; // Reset resize direction
                _selectedElement = null; // Reset selected element
            }
        }

        // Determine resize direction
        private ResizeDirection DetermineResizeDirection(UIElement element, Point point)
        {
            if (element is Shape shape)
            {
                Rect bounds = new Rect(
                    Canvas.GetLeft(shape),
                    Canvas.GetTop(shape),
                    shape.Width,
                    shape.Height
                );

                double padding = 10; // Capture area size

                if (point.Y < bounds.Top + padding)
                {
                    if (point.X < bounds.Left + padding)
                        return ResizeDirection.TopLeft;
                    else if (point.X > bounds.Right - padding)
                        return ResizeDirection.TopRight;
                    else
                        return ResizeDirection.Top;
                }
                else if (point.Y > bounds.Bottom - padding)
                {
                    if (point.X < bounds.Left + padding)
                        return ResizeDirection.BottomLeft;
                    else if (point.X > bounds.Right - padding)
                        return ResizeDirection.BottomRight;
                    else
                        return ResizeDirection.Bottom;
                }
                else if (point.X < bounds.Left + padding)
                {
                    return ResizeDirection.Left;
                }
                else if (point.X > bounds.Right - padding)
                {
                    return ResizeDirection.Right;
                }
            }

            return ResizeDirection.None;
        }

        // Resize an element
        private void ResizeElement(UIElement element, Point currentPoint)
        {
            if (element is Shape shape)
            {
                double left = Canvas.GetLeft(shape);
                double top = Canvas.GetTop(shape);
                double width = shape.Width;
                double height = shape.Height;

                switch (_resizeDirection)
                {
                    case ResizeDirection.TopLeft:
                        Canvas.SetLeft(shape, Math.Min(left, currentPoint.X));
                        Canvas.SetTop(shape, Math.Min(top, currentPoint.Y));
                        shape.Width = Math.Max(width + (left - currentPoint.X), 10);
                        shape.Height = Math.Max(height + (top - currentPoint.Y), 10);
                        break;
                    case ResizeDirection.Top:
                        Canvas.SetTop(shape, Math.Min(top, currentPoint.Y));
                        shape.Height = Math.Max(height + (top - currentPoint.Y), 10);
                        break;
                    case ResizeDirection.TopRight:
                        Canvas.SetTop(shape, Math.Min(top, currentPoint.Y));
                        shape.Width = Math.Max(Math.Abs(Canvas.GetLeft(shape) - currentPoint.X), 10);
                        shape.Height = Math.Max(height + (top - currentPoint.Y), 10);
                        break;
                    case ResizeDirection.Right:
                        shape.Width = Math.Max(Math.Abs(Canvas.GetLeft(shape) - currentPoint.X), 10);
                        break;
                    case ResizeDirection.BottomRight:
                        shape.Width = Math.Max(Math.Abs(Canvas.GetLeft(shape) - currentPoint.X), 10);
                        shape.Height = Math.Max(Math.Abs(Canvas.GetTop(shape) - currentPoint.Y), 10);
                        break;
                    case ResizeDirection.Bottom:
                        shape.Height = Math.Max(Math.Abs(Canvas.GetTop(shape) - currentPoint.Y), 10);
                        break;
                    case ResizeDirection.BottomLeft:
                        Canvas.SetLeft(shape, Math.Min(left, currentPoint.X));
                        shape.Width = Math.Max(width + (left - currentPoint.X), 10);
                        shape.Height = Math.Max(Math.Abs(Canvas.GetTop(shape) - currentPoint.Y), 10);
                        break;
                    case ResizeDirection.Left:
                        Canvas.SetLeft(shape, Math.Min(left, currentPoint.X));
                        shape.Width = Math.Max(width + (left - currentPoint.X), 10);
                        break;
                }
            }
        }

        // Move an element
        private void MoveElement(UIElement element, Point currentPoint)
        {
            double deltaX = currentPoint.X - _dragStartPoint.X;
            double deltaY = currentPoint.Y - _dragStartPoint.Y;
            if (element is Shape shape)
            {
                if (shape != null)
                {
                    Canvas.SetLeft(shape, Canvas.GetLeft(shape) + deltaX);
                    Canvas.SetTop(shape, Canvas.GetTop(shape) + deltaY);
                }
            }
            else if (element is Path path)
            {
                if (path != null)
                {
                    Canvas.SetLeft(path, Canvas.GetLeft(path) + deltaX);
                    Canvas.SetTop(path, Canvas.GetTop(path) + deltaY);
                }

            }
            _dragStartPoint = currentPoint;
        }

        // Enum for shape types
        public enum ShapeType
        {
            None,
            Rectangle,
            Circle,
            Snowman
        }

        // Enum for resize directions
        public enum ResizeDirection
        {
            None,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left
        }

        // Highlight selected element
        private void HighlightSelectedElement(UIElement element)
        {
            if (element is Shape shape)
            {
                shape.Stroke = new SolidColorBrush(Colors.Green); // Green border for selected figure
                shape.StrokeThickness = 4; // Increase border thickness
            }
        }

        // Unhighlight selected element
        private void UnhighlightSelectedElement(UIElement element)
        {
            if (element is Shape shape)
            {
                shape.Stroke = new SolidColorBrush(Colors.Black); // Restore black border
                shape.StrokeThickness = 2; // Restore default thickness
            }
        }

        // Remove selected element
        private void RemoveSelectedElement(object sender, RoutedEventArgs e)
        {
            if (_selectedElement != null)
            {
                DrawingCanvas.Children.Remove(_selectedElement);
                _drawnElements.Remove(_selectedElement);
                UnhighlightSelectedElement(_selectedElement);
                _selectedElement = null;
            }
        }

        // Select an element
        private void SelectElement(UIElement element)
        {
            if (_selectedElement != null)
            {
                UnhighlightSelectedElement(_selectedElement);
            }

            _selectedElement = element;
            HighlightSelectedElement(_selectedElement);
        }

        private void Canvas_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void SelectSnowman(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Меняем цвет Border на цвет выбранной кнопки
                CurrentColor.Background = button.Background;
                colorList.Hide();
            }
        }
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Обновляем TextBlock на выбранную цифру
                SelectedNumber.Text = button.Content.ToString();

                // Закрываем Flyout
                numberFlyout.Hide();
            }
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для кнопки "+"
            // Например, можно открыть диалог или добавить новую цифру
            SelectedNumber.Text = "+";

            // Закрываем Flyout
            numberFlyout.Hide();
        }
        private async void OpenColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            // Показываем ContentDialog
            var result = await colorPickerDialog.ShowAsync();

            // Если пользователь нажал "OK", обрабатываем выбранный цвет
            if (result == ContentDialogResult.Primary)
            {
                var selectedColor = colorPicker.Color;
                // Используйте selectedColor по своему усмотрению
            }
        }

        // Обработчик для кнопки "OK"
        private void ColorPickerDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Логика при нажатии на "OK"
        }

        // Обработчик для кнопки "Cancel"
        private void ColorPickerDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Логика при нажатии на "Cancel"
        }
    }
}