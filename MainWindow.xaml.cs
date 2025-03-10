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
using Microsoft.Web.WebView2.Core;
using Microsoft.UI.System;
using Microsoft.UI.Xaml.Controls.Primitives;
using static System.Net.WebRequestMethods;
using Windows.UI.Input.Inking;
using Windows.ApplicationModel.Store;

using static App3.StateMethods;
using static App3.CheckFunctions;
using static App3.PointerPressActions;
using static App3.PointerMoveActions;
using static App3.SelectionFunctions;
using static App3.LayerFunctions;
using static App3.ButtonCreator;
using static App3.CursorManager;

#endregion

namespace App3
{
    /// <summary>
    /// Класс, содержащий канвас с доступом к изменению курсора
    /// </summary>
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

        private LayerManager? _layerManager = new LayerManager();
        private Canvas? currentLayer;
        private int currentLayerIndex;
        private Canvas? previewLayer;

        internal Tool selectedTool = Tool.Brush;
        private Polyline? currentStroke;
        private Point startPoint;

        private Shape? selectedShape;
        private Rectangle? selectionRectangle;
        private Figure? previewFigure;

        private Color defaultCanvasColor = Colors.Transparent;
        private Color selectedColor = Colors.Black;
        private int selectedStrokeThickness = 2;

        private bool isDrawing = false;
        private bool isCreatingFigure = false;
        private bool isDragging = false;
        private bool isResizing = false;
        private bool isRotating = false;

        private string resizeDirection = "";

        private TextBox? inputTextBox;

        private Button? selectedButton;

        private Image? rotationHandle;

        private double defaultRotationAngle = 0;

        private double currentRotationAngle = 0;

        private Point previousPoint;

        internal static HashSet<Tool> availableShapes = [Tool.Rectangle, Tool.Circle, Tool.Triangle, Tool.RightTriangle, Tool.Rhombus, Tool.GoldenStar, Tool.Person, Tool.Line];
        
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
            InitializeLayers(ref currentLayerIndex, ref _layerManager, ref DrawingCanvas, defaultCanvasColor, Canvas_DoubleTapped);
            UpdateCanvasSizes(ref _layerManager, ref DrawingCanvas);
            SetActiveLayer(currentLayerIndex, ref _layerManager, ref DrawingCanvas, ref currentLayer);

            ChangeCursor(CursorStates.Default, ref DrawingCanvas);
        }

        #region TOOLS-REALIZATION

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
            Point currentPoint = e.GetCurrentPoint(DrawingCanvas).Position;

            startPoint = currentPoint;
            
            // drag / resize / remove selection
            if (selectedShape != null)
            {
                if (IsOnBorder(currentPoint, selectedShape, out resizeDirection))
                {
                    SetActive(ref isResizing);
                    SetInactive(ref isDrawing);
                    SetInactive(ref isDragging);
                    SetInactive(ref isCreatingFigure);
                    SetInactive(ref isRotating);
                    InitializePreviewLayer(ref previewLayer, ref DrawingCanvas, defaultCanvasColor);
                }
                else if (IsInCenter(currentPoint, selectedShape))
                {
                    SetActive(ref isRotating);
                    SetInactive(ref isDragging);
                    SetInactive(ref isDrawing);
                    SetInactive(ref isResizing);
                    SetInactive(ref isCreatingFigure);
                    InitializePreviewLayer(ref previewLayer, ref DrawingCanvas, defaultCanvasColor);
                }
                else if (IsPointInsideElement(currentPoint, selectedShape))
                {
                    SetActive(ref isDragging);
                    SetInactive(ref isDrawing);
                    SetInactive(ref isResizing);
                    SetInactive(ref isCreatingFigure);
                    SetInactive(ref isRotating);
                    InitializePreviewLayer(ref previewLayer, ref DrawingCanvas, defaultCanvasColor);
                }
                else
                {
                    RemoveSelection(ref selectionRectangle, ref currentLayer, ref selectedShape, ref rotationHandle);
                }
                return;
            }

            // brush / eraser
            if (selectedTool == Tool.Brush || selectedTool == Tool.Eraser)
            {
                StartDrawing(currentPoint, ref currentStroke, selectedColor, selectedStrokeThickness, selectedTool, ref currentLayer);
                SetActive(ref isDrawing);
            }
            // draw figure
            else if (IsFigure(selectedTool))
            {
                SetActive(ref isCreatingFigure);
                InitializePreviewLayer(ref previewLayer, ref DrawingCanvas, defaultCanvasColor);
            }
            // fill
            else if (selectedTool == Tool.Fill)
            {
                FillObject(ref e, new SolidColorBrush(selectedColor));
            }
            // color picker
            else if (selectedTool == Tool.SelectColorPicker)
            {
                PickColor(ref e, ref colorPicker);
            }
        }

        /// <summary>
        /// Метод, отслеживающий одиночное движение курсора
        /// </summary>
        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point currentPoint = e.GetCurrentPoint(DrawingCanvas).Position;

            ChangeCursorState(e, currentPoint);

            // draw
            if (IsActive(isDrawing))
            {
                Draw(currentPoint, ref currentStroke);
            }
            // draw figure
            else if (IsActive(isCreatingFigure))
            {
                DrawFigure(currentPoint, startPoint, selectedTool, defaultCanvasColor, selectedColor, ref previewFigure, ref previewLayer, defaultRotationAngle);
            }
            // rotate
            else if (IsActive(isRotating))
            {
                Rotate(ref previousPoint, currentPoint, startPoint, ref selectedShape, ref previewFigure, ref previewLayer, currentRotationAngle, ref currentLayer, ref selectionRectangle, ref rotationHandle);
            }
            // drag
            else if (IsActive(isDragging))
            {
                Drag(currentPoint, startPoint, ref selectedShape, ref previewLayer, ref currentLayer, ref previewFigure, ref selectionRectangle, ref rotationHandle);
            }
            // resize
            else if (IsActive(isResizing))
            {
                Resize(currentPoint, startPoint, ref selectedShape, resizeDirection, selectedTool, ref previewFigure, ref selectionRectangle, ref previewLayer, ref currentLayer, ref rotationHandle, currentRotationAngle);
            }
        }

        /// <summary>
        /// Функция, отвечающая за смену курсора
        /// </summary>
        /// <param name="e">Аргументы курсора</param>
        /// <param name="currentPoint">Текущее положение курсора</param>
        private void ChangeCursorState(PointerRoutedEventArgs e, Point currentPoint)
        {
            if (e.OriginalSource is Shape shape && !IsActive(isDrawing) && !IsActive(isCreatingFigure))
            {
                if (e.OriginalSource is Polyline)
                {
                    ChangeCursor(CursorStates.Default, ref DrawingCanvas);
                }
                else if (IsOnBorder(currentPoint, shape, out resizeDirection) && selectionRectangle != null)
                {
                    UpdateCursorForResizeDirection(resizeDirection, ref DrawingCanvas);
                }
                else if (IsPointInsideElement(currentPoint, shape))
                {
                    ChangeCursor(CursorStates.Default, ref DrawingCanvas);
                }
            }
            else
            {
                ChangeCursor(CursorStates.Drawing, ref DrawingCanvas);
            }
        }

        /// <summary>
        /// Метод, отслеживающий отжатие курсора
        /// </summary>
        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (currentLayer == null) return;

            if ((isDragging || isResizing || isRotating) && selectedShape != null)
            {
                DrawSelectionRectangle(selectedShape, ref selectionRectangle, ref currentLayer, ref rotationHandle, ref currentRotationAngle);
            }

            SetInactive(ref isDrawing);
            SetInactive(ref isDragging);
            SetInactive(ref isResizing);
            SetInactive(ref isCreatingFigure);

            if (previewFigure != null && currentLayer != null)
            {
                previewFigure.Draw(currentLayer);

                // чтобы только что нарисованная фигура не помечалась как выбранная
                if (selectedShape != null) selectedShape = currentLayer.Children.Last() as Shape;

                previewFigure = null;
                previewLayer?.Children.Clear();
            }
            DestroyPreviewLayer(ref previewLayer, ref DrawingCanvas);
        }

        /// <summary>
        /// Метод, отслеживающий двойное нажатие
        /// </summary>
        private void Canvas_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (e.OriginalSource is Shape shape && shape.Tag is Figure figure)
            {
                SetInactive(ref isDrawing);
                SetInactive(ref isResizing);
                SetInactive(ref isCreatingFigure);
                SetInactive(ref isRotating);
                SetActive(ref isDragging);

                currentRotationAngle = 0;
                selectedShape = shape;
                InitializePreviewLayer(ref previewLayer, ref DrawingCanvas, defaultCanvasColor);
                DrawSelectionRectangle(selectedShape, ref selectionRectangle, ref currentLayer, ref rotationHandle, ref currentRotationAngle);

                ChangeCursor(CursorStates.Default, ref DrawingCanvas);
            }
        }

        /// <summary>
        /// Метод, отслеживающий нажатие клавиши
        /// </summary>
        private void Canvas_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (selectionRectangle != null && e.Key == Windows.System.VirtualKey.Delete)
            {
                RemoveSelectedShapeFromLayer(selectedShape, ref currentLayer);
                RemoveSelection(ref selectionRectangle, ref currentLayer, ref selectedShape, ref rotationHandle);
                SetInactive(ref isDragging);
            }
        }
       
        /// <summary>
        /// Если курсор зашел на слой
        /// </summary>
        private void CustomCanvas_PointerEntered(object sender, PointerRoutedEventArgs e) => ChangeCursor(CursorStates.Drawing, ref DrawingCanvas);

        /// <summary>
        /// Если курсор покинул слой
        /// </summary>
        private void CustomCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Canvas_PointerReleased(sender, e);
            RemoveSelection(ref selectionRectangle, ref currentLayer, ref selectedShape, ref rotationHandle);
        }

        #region LAYERS

        /// <summary>
        /// Выбор номера слоя
        /// </summary>
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ResetLayerOrder(ref _layerManager, ref DrawingCanvas);
                SelectedNumber.Text = button.Content.ToString();

                if (_layerManager == null) return;

                if (string.IsNullOrEmpty(SelectedNumber.Text)) return;

                currentLayerIndex = int.Parse(SelectedNumber.Text) - 1;
                currentLayer = _layerManager.GetLayer(currentLayerIndex);
                SetActiveLayer(currentLayerIndex, ref _layerManager, ref DrawingCanvas, ref currentLayer);
                numberFlyout.Hide();
            }
        }

        /// <summary>
        /// Добавление слоя
        /// </summary>
        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            AddLayer(ref _layerManager, ref DrawingCanvas, ref previewLayer, defaultCanvasColor, Canvas_DoubleTapped);

            if (_layerManager == null) return;

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

            CreatePlusLayerButton(ref ButtonContainer, PlusButton_Click);
            CreateMinusLayerButton(ref ButtonContainer, MinusButton_Click);

            numberFlyout.Hide();
        }

        /// <summary>
        /// Удаление слоя
        /// </summary>
        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            int indexToRemove = currentLayerIndex;

            if (_layerManager == null) return;

            if (_layerManager.GetLayerCount() <= 1) return;

            if (int.Parse(SelectedNumber.Text) >= _layerManager.GetLayerCount())
            {
                SelectedNumber.Text = (currentLayerIndex).ToString();
            }
            if (int.Parse(SelectedNumber.Text) <= 0)
            {
                SelectedNumber.Text = "";
            }
            RemoveLayer(ref _layerManager, ref DrawingCanvas, ref currentLayerIndex, ref previewLayer, ref currentLayer, ref defaultCanvasColor, Canvas_DoubleTapped);

            ButtonContainer.Children.RemoveAt(indexToRemove);
            int startIndex = 1;
            foreach (Button button in ButtonContainer.Children)
            {
                if (button.Content.ToString() == "+" || button.Content.ToString() == "-") break;

                button.Content = startIndex++;
            }
        }
        
        /// <summary>
        /// Метод, срабатывающий при изменении размера канваса
        /// </summary>
        private void OnDrawingCanvasSizeChanged(object sender, SizeChangedEventArgs e) => UpdateCanvasSizes(ref _layerManager, ref DrawingCanvas);

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

        /// <summary>
        /// Обработчик нажатия кнопки "OK" в палитре
        /// </summary>
        private void ColorPickerDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) { }

        /// <summary>
        /// Обработчик нажатия кнопки "Cancel" в палитре
        /// </summary>
        private void ColorPickerDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) { }

        /// <summary>
        /// Выделяет выбранный инструмент в панели инструментов
        /// </summary>
        /// <param name="newSelected">Кнопка нового инструмента</param>
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

        #endregion

        #region FILE-PANEL

        /// <summary>
        /// Создание нового файла
        /// </summary>
        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            if (DrawingCanvas == null || _layerManager == null) return;
            DrawingCanvas.Children.Clear();
            _layerManager.ClearAllLayers();
        }

        /// <summary>
        /// Функция для открытия файла
        /// </summary>
        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            NewFile_Click(sender, e);

            if (_layerManager == null) return;

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
            ResetLayerOrder(ref _layerManager, ref DrawingCanvas);
            SetActiveLayer(currentLayerIndex, ref _layerManager, ref DrawingCanvas, ref currentLayer);
        }

        /// <summary>
        /// Функция для сохранения файла
        /// </summary>
        private async void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (_layerManager == null) return;

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
