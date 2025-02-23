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
                if (element is Shape shape)
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

            if (_isDrawing && _currentFigure != null)
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
                _currentFigure = null;
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
            if (element is Shape shape)
            {
                double deltaX = currentPoint.X - _dragStartPoint.X;
                double deltaY = currentPoint.Y - _dragStartPoint.Y;

                Canvas.SetLeft(shape, Canvas.GetLeft(shape) + deltaX);
                Canvas.SetTop(shape, Canvas.GetTop(shape) + deltaY);

                _dragStartPoint = currentPoint;
            }
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
            throw new NotImplementedException();
        }
    }
}