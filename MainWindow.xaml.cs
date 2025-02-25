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
        #region DEFINED VARIABLES

        private Random random = new Random();

        private LayerManager _layerManager = new LayerManager();

        private Tool selectedTool = Tool.Brush;
        private Polyline currentStroke;
        private bool isDrawing = false;
        private Point startPoint;
        private Figure previewFigure;

        private int currentLayerIndex;

        private Color defaultCanvasColor = Colors.Transparent;

        private bool isDragging = false;
        private Point lastPointerPosition;
        private UIElement selectedElement;
        private Rectangle selectionRectangle;

        private bool isResizing = false;
        private string resizeDirection = "";

        private Canvas previewLayer;
        private Color selectedColor = Colors.Black;
        private int selectedStrokeThickness = 2;


        private InputCursor? OriginalInputCursor { get; set; }

        private enum Tool
        {
            Brush,
            Fill,
            Eraser,
            SelectColorPicker,
            Rectangle,
            Circle
        }
        private CoreCursor currentCursor = new CoreCursor(CoreCursorType.Custom, 223123);
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DrawingCanvas.SizeChanged += OnDrawingCanvasSizeChanged;

            InitializeLayers();
            UpdateCanvasSizes();

            ChangeCursor(CursorStates.Default);
        }

        #region TOOLS REALIZATION

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
        /// Метод, позволяющий выбрать ластик
        /// </summary>
        private void SelectEraser(object sender, RoutedEventArgs e) => selectedTool = Tool.Eraser;

        /// <summary>
        /// Метод, позволяющий выбрать пипетку
        /// </summary>
        private void SelectColorPicker(object sender, RoutedEventArgs e) => selectedTool = Tool.SelectColorPicker;

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
                }
                colorPicker.Color = selectedColor;
            }
            else if (selectedTool == Tool.Fill && selectedElement != null)
            {
                if (IsPointInsideElement(point, selectedElement) && selectedElement is Shape)
                {
                    Shape selectedShape = selectedElement as Shape;

                    if (selectedShape != null)
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
            if (selectedElement != null && selectedElement is Shape)
            {
                var point = e.GetCurrentPoint(DrawingCanvas).Position;
                string direction;
                if (IsOnBorder(point, selectedElement as Shape, out direction))
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

            if ((selectedTool == Tool.Eraser || selectedTool == Tool.Brush) && currentStroke != null)
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
                    previewFigure = new RectangleFigure(x, y, width, height, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(selectedColor), 2);
                }
                else if (selectedTool == Tool.Circle)
                {
                    previewFigure = new CircleFigure(x + width / 2, y + height / 2, Math.Max(width, height), new SolidColorBrush(Colors.Transparent), new SolidColorBrush(selectedColor), 2);
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
                previewFigure.Draw(_layerManager.GetLayer(currentLayerIndex));
                previewFigure = null;
                previewLayer.Children.Clear();
            }
            DestroyPreviewLayer();
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


        #region ADDITIONAL FUNCS

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

        #endregion

        #region BOOL STATE METHODS

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
        /// Метод, включающий режим рисования
        /// </summary>
        private void MakeDrawingInactive() => isDrawing = false;

        /// <summary>
        ///  Метод, выключающий режим рисования
        /// </summary>
        private void MakeDrawingActive() => isDrawing = true;

        #endregion

        #region SELECTION METHODS

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
        private void RemoveSelectionRectangle(ref Rectangle selectionRectangle)
        {
            _layerManager.GetLayer(currentLayerIndex).Children.Remove(selectionRectangle);
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
        private void RemoveSelectedElement(object sender, RoutedEventArgs e)
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

        #region Layers
        private void AddLayerButton_Click(object sender, RoutedEventArgs e)
        {
            _layerManager.AddLayer();
            _layerManager.GetLayer(_layerManager.GetLayerCount() - 1).Background = new SolidColorBrush(defaultCanvasColor);
            DrawingCanvas.Children.Remove(previewLayer);
            DrawingCanvas.Children.Add(_layerManager.GetLayer(_layerManager.GetLayerCount() - 1));
            previewLayer = new Canvas();
            DrawingCanvas.Children.Add(previewLayer);

            UpdateCanvasSizes();
        }
        private void RemoveLayerButton_Click(object sender, RoutedEventArgs e)
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
        }

        private void BringToFrontButton_Click(object sender, RoutedEventArgs e)
        {
            int activeLayerIndex = GetCurrentActiveLayerIndex(); // Предполагается функция получения активного слоя
            _layerManager.BringToFront(activeLayerIndex);

            ReorderLayersInDrawingCanvas();
        }

        private void SendToBackButton_Click(object sender, RoutedEventArgs e)
        {
            int activeLayerIndex = GetCurrentActiveLayerIndex(); // Предполагается функция получения активного слоя
            _layerManager.SendToBack(activeLayerIndex);

            ReorderLayersInDrawingCanvas();
        }

        private void ReorderLayersInDrawingCanvas()
        {
            DrawingCanvas.Children.Clear();
            foreach (var layer in _layerManager.GetAllLayers())
            {
                DrawingCanvas.Children.Add(layer);
            }
        }

        private int GetCurrentActiveLayerIndex() => currentLayerIndex;

        private void GetRandomColor_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = Color.FromArgb(255, (byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));
        }

        private void InitializeLayers()
        {
            currentLayerIndex = 0;

            _layerManager.AddLayer();


            foreach (var layer in _layerManager.GetAllLayers())
            {
                layer.Background = new SolidColorBrush(defaultCanvasColor);
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

        private void SelectNextLayer_Click(object sender, RoutedEventArgs e)
        {
            if (currentLayerIndex >= _layerManager.GetLayerCount() - 1)
            {
                currentLayerIndex = _layerManager.GetLayerCount() - 1;
                return;
            }
            currentLayerIndex++;
            Debug.Print(currentLayerIndex.ToString());
        }

        private void SelectPreviousLayer_Click(object sender, RoutedEventArgs e)
        {
            if (currentLayerIndex <= 0)
            {
                currentLayerIndex = 0;
                return;
            }
            currentLayerIndex--;
            Debug.Print(currentLayerIndex.ToString());
        }

        private void InitializePreviewLayer()
        {
            previewLayer = new Canvas();
            previewLayer.Background = new SolidColorBrush(defaultCanvasColor);
            DrawingCanvas.Children.Add(previewLayer);
        }

        private void DestroyPreviewLayer()
        {
            if (previewLayer != null)
            {
                DrawingCanvas.Children.Remove(previewLayer);
            }
            previewLayer = null;
        }

        #endregion

        #region CURSOR STUFF
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

        private void CustomCanvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is CustomCanvas canvas)
            {
                ChangeCursor(CursorStates.Drawing);
            }
        }

        private void CustomCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is CustomCanvas canvas)
            {
                ChangeCursor(CursorStates.Default);
            }
        }
        #endregion


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
            var button = sender as Button;

            if (button != null)
            {
                selectedColor = ((SolidColorBrush)button.Background).Color;

                CurrentColor.Background = new SolidColorBrush(selectedColor);

                myColorButton.Flyout.Hide();

                colorPicker.Color = selectedColor;
            }
        }
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SelectedNumber.Text = button.Content.ToString();

                if (string.IsNullOrEmpty(SelectedNumber.Text)) return;

                currentLayerIndex = int.Parse(SelectedNumber.Text);

                numberFlyout.Hide();
            }
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            // ������ ��� ������ "+"
            // ��������, ����� ������� ������ ��� �������� ����� �����
            SelectedNumber.Text = "+";

            // ��������� Flyout
            numberFlyout.Hide();
        }
        private async void OpenColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await colorPickerDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                selectedColor = colorPicker.Color;
            }
        }

        // ���������� ��� ������ "OK"
        private void ColorPickerDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // ������ ��� ������� �� "OK"
        }

        // ���������� ��� ������ "Cancel"
        private void ColorPickerDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // ������ ��� ������� �� "Cancel"
        }


        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            // Ожидаем выбора файла
            var file = await picker.PickSingleFileAsync();
            if (file == null) return;  // Если файл не выбран, выходим

            // Открываем поток для чтения файла
            var stream = await file.OpenAsync(FileAccessMode.Read);

            // Создаем BitmapImage и загружаем в него данные из потока
            BitmapImage bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);  // Асинхронно загружаем изображение

            // Создаем элемент Image и добавляем его на холст
            Image image = new Image { Source = bitmap };
            DrawingCanvas.Children.Clear();  // Очищаем холст
            _layerManager.ClearAllLayers();
            _layerManager.AddLayer();
            currentLayerIndex = 0;
            _layerManager.GetLayer(currentLayerIndex).Children.Add(image);  // Добавляем изображение
            DrawingCanvas.Children.Add(_layerManager.GetLayer(currentLayerIndex));
            UpdateCanvasSizes();
        }

        private async void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("PNG Image", new[] { ".png" });
            picker.SuggestedFileName = "Рисунок";

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            // Создаем поток для записи в файл
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);

            // Рендерим изображение в Bitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap();
            await renderBitmap.RenderAsync(DrawingCanvas); // Асинхронно

            // Получаем данные пикселей и записываем в поток
            var pixels = await renderBitmap.GetPixelsAsync();

            // Используем BitmapEncoder для сохранения изображения в PNG
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)renderBitmap.PixelWidth, (uint)renderBitmap.PixelHeight, 96, 96, pixels.ToArray());

            // Закрываем поток после записи
            await encoder.FlushAsync();
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void StrokeThickness_Changed(object sender, RoutedEventArgs e)
        {
            selectedStrokeThickness = (int)slider.Value;
        }
    }
}
