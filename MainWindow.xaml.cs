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
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.UI.Dispatching;
using Windows.Devices.Input;
using Microsoft.UI.Input;
using Windows.UI.Core;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using static App3.StateMethods;
using Microsoft.Web.WebView2.Core;
using Microsoft.UI.System;
using Microsoft.UI.Xaml.Controls.Primitives;
using static System.Net.WebRequestMethods;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace App3
{
    public class CustomCanvas : Canvas
    {
        public InputCursor InputCursor
        {
            get => ProtectedCursor;
            set => ProtectedCursor = value;
        }
    }
    public sealed partial class MainWindow : Window
    {
        #region DEFINED-VARIABLES

        private Random random = new Random();

        private LayerManager _layerManager = new LayerManager();

        internal Tool selectedTool = Tool.Brush;
        private Polyline? currentStroke;
        private bool isDrawing = false;
        private Point startPoint;
        private Figure? previewFigure;

        private int currentLayerIndex;

        private Color defaultCanvasColor = Colors.Transparent;

        private bool isDragging = false;
        private Point lastPointerPosition;
        private UIElement? selectedElement;
        private Rectangle? selectionRectangle;

        private bool isResizing = false;
        private string resizeDirection = "";

        private Canvas? previewLayer;
        private Color selectedColor = Colors.Black;
        private int selectedStrokeThickness = 2;

        private TextBox inputTextBox;

        internal enum Tool
        {
            Brush,
            Fill,
            Eraser,
            SelectColorPicker,
            Rectangle,
            Circle,
            Triangle,
            RightTriangle,
            Rhombus,
            GoldenStar,
            Person,
            Line,
            Text
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DrawingCanvas.SizeChanged += OnDrawingCanvasSizeChanged;
            InitializeLayers();
            UpdateCanvasSizes();

            ChangeCursor(CursorStates.Default);
        }

        #region TOOLS-REALIZATION

        private Button? selectedButton = null;

        /// <summary>
        /// Метод, позволяющий выбрать кисть
        /// </summary>
        private void SelectBrush(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Brush;
            SetSelectedButton(Brush);
        }

        /// <summary>
        /// Метод, позволяющий выбрать прямоугольник
        /// </summary>
        private void SelectRectangle(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Rectangle;
            SetSelectedButton(Rectangle);
        }

        /// <summary>
        /// Метод, позволяющий выбрать круг
        /// </summary>
        private void SelectCircle(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Circle;
            SetSelectedButton(Circle);
        }

        /// <summary>
        /// Метод, позволяющий выбрать треугольник
        /// </summary>
        private void SelectTriangle(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Triangle;
            SetSelectedButton(Triangle);
        }

        /// <summary>
        /// Метод, позволяющий выбрать прямоугольный треугольник
        /// </summary>
        private void SelectRightTriangle(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.RightTriangle;
            SetSelectedButton(RightTriangle);
        }

        /// <summary>
        /// Метод, позволяющий выбрать ромб
        /// </summary>
        private void SelectRhombus(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Rhombus;
            SetSelectedButton(Rhomb);
        }

        /// <summary>
        /// Метод, позволяющий выбрать звезду
        /// </summary>
        private void SelectGoldenStar(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.GoldenStar;
            SetSelectedButton(GoldenStar);
        }

        /// <summary>
        /// Метод, позволяющий выбрать человечка
        /// </summary>
        private void SelectPerson(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Person;
            SetSelectedButton(Person);
        }

        /// <summary>
        /// Метод, позволяющий выбрать заливку
        /// </summary>
        private void SelectFill(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Fill;
            SetSelectedButton(Fill);
        }

        /// <summary>
        /// Метод, позволяющий выбрать ластик
        /// </summary>
        private void SelectEraser(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Eraser;
            SetSelectedButton(Eraser);
        }

        /// <summary>
        /// Метод, позволяющий выбрать пипетку
        /// </summary>
        private void SelectColorPicker(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.SelectColorPicker;
            SetSelectedButton(ColorPicker);
        }

        /// <summary>
        /// Метод, позволяющий выбрать линию
        /// </summary>
        private void SelectLine(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Line;
            SetSelectedButton(Line);
        }

        /// <summary>
        /// Метод, позволяющий выбрать текст
        /// </summary>
        private void SelectText(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Text;
            SetSelectedButton(Text);
        }

        /// <summary>
        /// Метод для очищения канваса
        /// </summary>
        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            var currentLayer = _layerManager.GetLayer(currentLayerIndex);

            if (currentLayer == null) return;

            currentLayer.Children.Clear();
            currentLayer.Background = new SolidColorBrush(defaultCanvasColor);
        }

        #endregion

        #region POINTER-EVENTS

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
                    MakeActive(ref isResizing);
                    return;
                }
                else if (selectedElement != null && selectedElement is Shape && IsPointInsideElement(point, selectedElement))
                {
                    MakeActive(ref isDragging);
                }
                else
                {
                    RemoveSelection();
                    MakeInactive(ref isDragging);
                }
                isDrawing = false;
                lastPointerPosition = point;
                return;
            }
            selectedElement = e.OriginalSource as UIElement;

            InitializePreviewLayer();

            if (selectedTool == Tool.Brush || selectedTool == Tool.Eraser)
            {
                currentStroke = new Polyline
                {
                    Stroke = new SolidColorBrush(selectedColor),
                    StrokeThickness = selectedStrokeThickness
                };
                if (selectedTool == Tool.Eraser)
                {
                    currentStroke.Stroke = new SolidColorBrush(Colors.White);
                }
                currentStroke.Points.Add(point);
                _layerManager.GetLayer(currentLayerIndex).Children.Add(currentStroke);
            }
            else if (selectedTool == Tool.SelectColorPicker) 
            {
                if (selectedElement != null && selectedElement is Shape shape)
                {
                    selectedColor = ((SolidColorBrush)shape.Fill).Color;
                }
                else if (selectedElement != null && selectedElement is Canvas canvas)
                {
                    selectedColor = ((SolidColorBrush)canvas.Background).Color;
                    if (selectedColor == Colors.Transparent)
                    {
                        selectedColor = Colors.White;
                    }
                }
                colorPicker.Color = selectedColor;
            }
            else if (selectedTool == Tool.Fill && selectedElement != null)
            {
                if (selectedElement is Shape)
                {
                    if (selectedElement != null && selectedElement is Shape selectedShape)
                    {
                        selectedShape.Fill = new SolidColorBrush(selectedColor);
                    }
                }
                else if (selectedElement is Canvas)
                {
                    if (_layerManager != null) _layerManager.GetLayer(currentLayerIndex).Background = new SolidColorBrush(selectedColor);
                    UpdateCanvasSizes();
                }
            }
        }

        /// <summary>
        /// Метод, отслеживающий одиночное движение курсора
        /// </summary>
        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (selectedElement != null && selectedElement is Shape selectedShape)
            {
                var point = e.GetCurrentPoint(DrawingCanvas).Position;
                string direction;
                if (IsOnBorder(point, selectedShape, out direction))
                {
                    if (!string.IsNullOrEmpty(direction))
                    {
                        UpdateCursorForResizeDirection(direction);
                    }
                }
                else
                {
                    ChangeCursor(CursorStates.Drawing);
                }
            }
            else
            {
                ChangeCursor(CursorStates.Drawing);
            }

            if (isResizing && !string.IsNullOrEmpty(resizeDirection))
            {
                var point = e.GetCurrentPoint(DrawingCanvas).Position;
                double dx = point.X - startPoint.X;
                double dy = point.Y - startPoint.Y;

                if (selectedElement != null && selectedElement is Shape shape)
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
                RemoveSelectionRectangle(ref selectionRectangle);

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

            if ((selectedTool == Tool.Eraser || selectedTool == Tool.Brush) && currentStroke != null)
            {
                currentStroke.Points.Add(pointMove);
            }
            else if (selectedTool == Tool.Text)
            {
                double x = Math.Min(startPoint.X, pointMove.X);
                double y = Math.Min(startPoint.Y, pointMove.Y);
                double width = Math.Abs(pointMove.X - startPoint.X);
                double height = Math.Abs(pointMove.Y - startPoint.Y);
                inputTextBox = new TextBox
                {
                    Width = width,
                    Height = height,
                    Background = new SolidColorBrush(Colors.Transparent),
                    Foreground = new SolidColorBrush(selectedColor),
                    BorderThickness = new Thickness(0),
                    FontSize = 20,
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap
                };
                Canvas.SetLeft(inputTextBox, x);
                Canvas.SetTop(inputTextBox, y);

                previewLayer?.Children.Clear();
                previewLayer?.Children.Add(inputTextBox);
                inputTextBox.TextChanged += InputTextBox_TextChanged;
                inputTextBox.LostFocus += InputTextBox_LostFocus;
                inputTextBox.Focus(FocusState.Programmatic);
            }
            else if (IsFigure())
{
                double x = Math.Min(startPoint.X, pointMove.X);
                double y = Math.Min(startPoint.Y, pointMove.Y);
                double width = Math.Abs(pointMove.X - startPoint.X);
                double height = Math.Abs(pointMove.Y - startPoint.Y);

                if (selectedTool == Tool.Rectangle)
                {
                    previewFigure = new RectangleFigure(x, y, width, height, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(selectedColor), 2);
                }
                else if (selectedTool == Tool.Circle)
                {
                    previewFigure = new CircleFigure(x + width / 2, y + height / 2, Math.Max(width, height), new SolidColorBrush(Colors.Transparent), new SolidColorBrush(selectedColor), 2);
                }
                else if (selectedTool == Tool.Line)
                {
                    previewFigure = new LineFigure(startPoint.X, startPoint.Y, pointMove.X, pointMove.Y, new SolidColorBrush(selectedColor), 2);
                }
                else if (selectedTool == Tool.Triangle)
                {
                    previewFigure = new TriangleFigure(x, y, width, height, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(selectedColor), 2);
                }
                else if (selectedTool == Tool.RightTriangle)
                {
                    previewFigure = new RightTriangleFigure(x, y, width, height, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(selectedColor), 2);
                }
                else if (selectedTool == Tool.Rhombus)
                {
                    previewFigure = new RhombusFigure(x, y, width, height, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(selectedColor), 2);
                }
                else if (selectedTool == Tool.GoldenStar)
                {
                    previewFigure = new GoldenStarFigure(x + width / 2, y + height / 2, Math.Max(width, height), new SolidColorBrush(Colors.Transparent), new SolidColorBrush(selectedColor), 2);
                }
                else if (selectedTool == Tool.Person)
                {
                    previewFigure = new PersonFigure(x, y, width, height, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(selectedColor), 2);
                }

                previewLayer?.Children.Clear();

                if (previewLayer != null) previewFigure?.Draw(previewLayer);
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

            MakeInactive(ref isDrawing);
            MakeInactive(ref isDragging);
            MakeInactive(ref isResizing);

            if (previewFigure != null)
            {
                previewFigure.Draw(_layerManager.GetLayer(currentLayerIndex));
                previewFigure = null;
                previewLayer?.Children.Clear();
            }
            DestroyPreviewLayer();
        }

        /// <summary>
        /// Метод, отслеживающий двойное нажатие
        /// </summary>
        private void Canvas_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            selectedElement = e.OriginalSource as UIElement; 
            if (selectedElement != null && selectedElement is Shape)
            {
                DrawSelectionRectangle(selectedElement);
                MakeActive(ref isDragging);
            }
        }

        #endregion

        #region TEXTBOXES

        /// <summary>
        /// Обработка изменение текста в текстбоксе
        /// </summary>
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputTextBox == null) return;

            inputTextBox.Width = inputTextBox.Width + 11;
        }

        /// <summary>
        /// Обработка потери фокуса текстбоксом
        /// </summary>
        private void InputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (inputTextBox == null || startPoint == null) return;

            var textBlock = new TextBlock
            {
                Text = inputTextBox.Text,
                FontSize = 20,
                Foreground = new SolidColorBrush(selectedColor)
            };

            Canvas.SetLeft(textBlock, startPoint.X);
            Canvas.SetTop(textBlock, startPoint.Y);

            _layerManager.GetLayer(currentLayerIndex).Children.Add(textBlock);

            _layerManager.GetLayer(currentLayerIndex).Children.Remove(inputTextBox);
            inputTextBox = null;
        }

        #endregion

        #region ADDITIONAL-FUNCS

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
                MakeInactive(ref isDragging);
            }
        }

        private void SetSelectedButton(Button newSelected)
        {
            // Сбрасываем предыдущую кнопку, если она есть
            if (selectedButton != null)
            {
                selectedButton.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            // Устанавливаем новую кнопку и выделяем её границу
            selectedButton = newSelected;
            selectedButton.BorderBrush = new SolidColorBrush(Colors.Blue);
        }

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

            if (Math.Abs(point.X - left) < 8 && point.Y <= bottom && point.Y >= top) direction += "left";
            if (Math.Abs(point.X - right) < 8 && point.Y <= bottom && point.Y >= top) direction += "right";
            if (Math.Abs(point.Y - top) < 8 && point.X >= left && point.X <= right) direction += "top";
            if (Math.Abs(point.Y - bottom) < 8 && point.X >= left && point.X <= right) direction += "bottom";

            return !string.IsNullOrEmpty(direction);
        }

        /// <summary>
        /// Проверка, является ли выбранный инструмент фигурой
        /// </summary>
        /// <returns>True - если выбранный инструмент является фигурой, иначе false</returns>
        private bool IsFigure() => selectedTool == Tool.Rectangle || selectedTool == Tool.Circle || selectedTool == Tool.Line ||
                                   selectedTool == Tool.Triangle || selectedTool == Tool.RightTriangle || selectedTool == Tool.Rhombus ||
                                   selectedTool == Tool.GoldenStar || selectedTool == Tool.Person;

        #endregion

        #region SELECTION-METHODS

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

                _layerManager.GetLayer(currentLayerIndex).Children.Add(selectionRectangle);
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
        /// Метод, удаляющий прямоугольник, отвечающий за выделение фигуры
        /// </summary>
        /// <param name="selectionRectangle">Выделяющий прямоугольник</param>
        private void RemoveSelectionRectangle(ref Rectangle? selectionRectangle)
        {
            if (selectionRectangle != null)
            {
                _layerManager.GetLayer(currentLayerIndex).Children.Remove(selectionRectangle);
            }
            selectionRectangle = null;
        }

        /// <summary>
        /// Метод, удаляющий выбранную фигуру
        /// </summary>
        private void RemoveSelectedElement()
        {
            if (selectedElement != null)
            {
                DrawingCanvas.Children.Remove(selectedElement);
                _layerManager.GetLayer(currentLayerIndex).Children.Remove(selectedElement);
            }
        }

        /// <summary>
        /// Метод, выполняющий сброс выбранной фигуры
        /// </summary>
        private void SetSelectedElementToNull() => selectedElement = null;

        #endregion

        #region LAYERS

        /// <summary>
        /// Добавление слоя
        /// </summary>
        private void AddLayer()
        {
            _layerManager.AddLayer();
            _layerManager.GetLayer(_layerManager.GetLayerCount() - 1).Background = new SolidColorBrush(defaultCanvasColor);
            DrawingCanvas.Children.Remove(previewLayer);
            DrawingCanvas.Children.Add(_layerManager.GetLayer(_layerManager.GetLayerCount() - 1));
            _layerManager.GetLayer(_layerManager.GetLayerCount() - 1).DoubleTapped += Canvas_DoubleTapped;
            previewLayer = new Canvas();
            DrawingCanvas.Children.Add(previewLayer);

            UpdateCanvasSizes();
        }
        
        /// <summary>
        /// Удаление слоя
        /// </summary>
        private void RemoveLayer()
        {
            int lastLayerIndex = _layerManager.GetLayerCount() - 1;
            if (lastLayerIndex >= 0)
            {
                _layerManager.RemoveLayer(lastLayerIndex);
                DrawingCanvas.Children.RemoveAt(lastLayerIndex);
            }

            if (currentLayerIndex == lastLayerIndex)
            {
                currentLayerIndex = _layerManager.GetLayerCount() - 1;
                if (currentLayerIndex < 0)
                {
                    currentLayerIndex = 0;
                }
            }

            if (_layerManager.GetLayerCount() == 0)
            {
                AddLayer();
            }
        }

        /// <summary>
        /// Инициализация слоев
        /// </summary>
        private void InitializeLayers()
        {
            currentLayerIndex = 0;
            _layerManager.AddLayer();

            foreach (var layer in _layerManager.GetAllLayers())
            {
                layer.Background = new SolidColorBrush(defaultCanvasColor);
                layer.DoubleTapped += Canvas_DoubleTapped;
                DrawingCanvas.Children.Add(layer);
            }
        }

        /// <summary>
        /// Метод, срабатывающий при изменении размера канваса
        /// </summary>
        private void OnDrawingCanvasSizeChanged(object sender, SizeChangedEventArgs e) => UpdateCanvasSizes();

        /// <summary>
        /// Метод, обновляющий размеры слоев, в соответствие с размерами основного канваса
        /// </summary>
        private void UpdateCanvasSizes()
        {
            if (DrawingCanvas != null)
            {
                foreach (Canvas layer in _layerManager.GetAllLayers())
                {
                    layer.Width = DrawingCanvas.ActualWidth;
                    layer.Height = DrawingCanvas.ActualHeight;
                }
            }
        }

        /// <summary>
        /// Инициализация preview-слоя
        /// </summary>
        private void InitializePreviewLayer()
        {
            previewLayer = new Canvas();
            previewLayer.Background = new SolidColorBrush(defaultCanvasColor);
            DrawingCanvas.Children.Add(previewLayer);
        }

        /// <summary>
        /// Уничтожение preview-слоя
        /// </summary>
        private void DestroyPreviewLayer()
        {
            if (previewLayer != null)
            {
                DrawingCanvas.Children.Remove(previewLayer);
            }
            previewLayer = null;
        }

        /// <summary>
        /// Выбор номера слоя
        /// </summary>
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ResetLayerOrder();
                SelectedNumber.Text = button.Content.ToString();

                if (string.IsNullOrEmpty(SelectedNumber.Text)) return;

                currentLayerIndex = int.Parse(SelectedNumber.Text) - 1;
                SetActiveLayer(currentLayerIndex);
                numberFlyout.Hide();
            }
        }

        /// <summary>
        /// Добавление слоя
        /// </summary>
        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            AddLayer();

            int lastLayerIndex = _layerManager.GetLayerCount() - 1;

            ButtonContainer.Children.RemoveAt(lastLayerIndex);
            ButtonContainer.Children.RemoveAt(lastLayerIndex);

            Button newNumberButton = new Button
            {
                Content = lastLayerIndex + 1,
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 0, 0)
            };
            newNumberButton.Click += NumberButton_Click;
            ButtonContainer.Children.Add(newNumberButton);

            if (SelectedNumber.Text == "") SelectedNumber.Text = (currentLayerIndex + 1).ToString();

            CreatePlusLayerButton();
            CreateMinusLayerButton();

            numberFlyout.Hide();
        }

        /// <summary>
        /// Создание кнопки "+"
        /// </summary>
        private void CreatePlusLayerButton()
        {
            Button plusButton = new Button
            {
                Content = '+',
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 0, 0)
            };
            plusButton.Click += PlusButton_Click;
            ButtonContainer.Children.Add(plusButton);
        }

        /// <summary>
        /// Создание кнопки "-"
        /// </summary>
        private void CreateMinusLayerButton()
        {
            Button minusButton = new Button
            {
                Content = '-',
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 0, 0)
            };
            minusButton.Click += MinusButton_Click;
            ButtonContainer.Children.Add(minusButton);
        }

        /// <summary>
        /// Удаление слоя
        /// </summary>
        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            int indexToRemove = currentLayerIndex;
            if (_layerManager.GetLayerCount() <= 1) return;

            if (int.Parse(SelectedNumber.Text) >= _layerManager.GetLayerCount())
            {
                SelectedNumber.Text = (currentLayerIndex).ToString();
            }
            if (int.Parse(SelectedNumber.Text) <= 0)
            {
                SelectedNumber.Text = "";
            }
            RemoveLayer();
            ButtonContainer.Children.RemoveAt(indexToRemove);
            int startIndex = 1;
            foreach (Button button in ButtonContainer.Children)
            {
                if (button.Content.ToString() == "+" || button.Content.ToString() == "-") break;

                button.Content = startIndex++;
            }
        }

        /// <summary>
        /// Назначает активный слой
        /// </summary>
        /// <param name="index">Индекс текущего слоя</param>
        private void SetActiveLayer(int index)
        {
            currentLayerIndex = index;
            Canvas activeLayer = _layerManager.GetLayer(index);
            DrawingCanvas.Children.Remove(activeLayer);
            DrawingCanvas.Children.Add(activeLayer);
        }

        /// <summary>
        /// Возвращает правильный порядок слоев
        /// </summary>
        private void ResetLayerOrder()
        {
            List<CustomCanvas> layers = _layerManager.GetAllLayers().ToList();
            DrawingCanvas.Children.Clear();

            foreach (var layer in layers)
            {
                DrawingCanvas.Children.Add(layer);
            }
        }

        #endregion

        #region CURSOR-STUFF
        enum CursorStates
        {
            Default,
            Drawing,
            Selecting,
            SizeNorthwestSoutheast,
            SizeNortheastSouthwest,
            SizeWestEast,
            SizeNorthSouth
        }
        private void ChangeCursor(CursorStates state)
        {
            switch (state)
            {
                case CursorStates.Default:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                    break;
                case CursorStates.Drawing:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.Cross);
                    break;
                case CursorStates.Selecting:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
                    break;
                case CursorStates.SizeNorthwestSoutheast:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthwestSoutheast);
                    break;
                case CursorStates.SizeNortheastSouthwest:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNortheastSouthwest);
                    break;
                case CursorStates.SizeWestEast:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
                    break;
                case CursorStates.SizeNorthSouth:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthSouth);
                    break;
            }
        }

        private void UpdateCursorForResizeDirection(string direction)
        {
            if (string.IsNullOrEmpty(direction)) return;

            switch (direction)
            {
                case "right":
                    ChangeCursor(CursorStates.SizeWestEast);
                    break;
                case "left":
                    ChangeCursor(CursorStates.SizeWestEast);
                    break;
                case "bottom":
                    ChangeCursor(CursorStates.SizeNorthSouth);
                    break;
                case "top":
                    ChangeCursor(CursorStates.SizeNorthSouth);
                    break;
                case "bottom-right":
                    ChangeCursor(CursorStates.SizeNorthwestSoutheast);
                    break;
                case "top-left":
                    ChangeCursor(CursorStates.SizeNorthwestSoutheast);
                    break;
                case "top-right":
                    ChangeCursor(CursorStates.SizeNortheastSouthwest);
                    break;
                case "bottom-left":
                    ChangeCursor(CursorStates.SizeNortheastSouthwest);
                    break;
                default:
                    ChangeCursor(CursorStates.Default);
                    break;
            }
        }
        private void CustomCanvas_PointerEntered(object sender, PointerRoutedEventArgs e) => ChangeCursor(CursorStates.Drawing);

        private void CustomCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Canvas_PointerReleased(sender, e);
            RemoveSelection();
        }
        #endregion

        #region DRAWING-PARAMETERS

        /// <summary>
        /// Выбор цвета в панели
        /// </summary>
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                selectedColor = ((SolidColorBrush)button.Background).Color;

                CurrentColor.Background = new SolidColorBrush(selectedColor);

                myColorButton.Flyout.Hide();

                colorPicker.Color = selectedColor;
            }
        }

        /// <summary>
        /// Открытие цветовой панели
        /// </summary>
        private async void OpenColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await colorPickerDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                selectedColor = colorPicker.Color;
            }
        }

        /// <summary>
        /// Изменение толщины кисти
        /// </summary>
        private void StrokeThickness_Changed(object sender, RoutedEventArgs e) => selectedStrokeThickness = (int)slider.Value;

        // "OK"
        private void ColorPickerDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) { }

        // "Cancel"
        private void ColorPickerDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) { }

        #endregion

        #region FILE-PANEL

        /// <summary>
        /// Создание нового файла
        /// </summary>
        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();
            _layerManager.ClearAllLayers();
        }

        /// <summary>
        /// Функция для открытия файла
        /// </summary>
        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            NewFile_Click(sender, e);

            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".json");
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return;

            currentLayerIndex = 0;

            _layerManager.ClearAllLayers();
            List<CustomCanvas> canvasList = new List<CustomCanvas>();
            canvasList = CanvasSerializer.LoadCanvasFromJson(file.Path);
            foreach (CustomCanvas canvas in canvasList)
            {
                canvas.DoubleTapped += Canvas_DoubleTapped;
                _layerManager.AddLayer(canvas);
                DrawingCanvas.Children.Add(canvas);
            }
            ResetLayerOrder();
            SetActiveLayer(currentLayerIndex);
        }

        /// <summary>
        /// Функция для сохранения файла
        /// </summary>
        private async void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JSON File", new[] { ".json" });
            picker.SuggestedFileName = "canvas";

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSaveFileAsync();
            
            if (file != null) CanvasSerializer.SaveCanvasToJson(_layerManager, file.Path);
        }

        #endregion
    }
}
