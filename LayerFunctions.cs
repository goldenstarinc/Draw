using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace App3
{
    /// <summary>
    /// Модуль, содержащий функции для работы со слоями
    /// </summary>
    internal static class LayerFunctions
    {

        /// <summary>
        /// Добавляет слой в объект класса отвечающий за работу со слоями
        /// </summary>
        /// <param name="_layerManager">Объект класса отвечающий за работу со слоями</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        /// <param name="previewLayer">Слой предпросмотра</param>
        /// <param name="defaultCanvasColor">Стандартный цвет слоя</param>
        /// <param name="Canvas_DoubleTapped">Обработчик события двойного нажатия</param>
        internal static void AddLayer(ref LayerManager? _layerManager, ref CustomCanvas? DrawingCanvas, ref Canvas? previewLayer, Color defaultCanvasColor, DoubleTappedEventHandler Canvas_DoubleTapped)
        {
            if (_layerManager == null || DrawingCanvas == null) return;

            _layerManager.AddLayer();
            _layerManager.GetLayer(_layerManager.GetLayerCount() - 1).Background = new SolidColorBrush(defaultCanvasColor);
            DrawingCanvas.Children.Remove(previewLayer);
            DrawingCanvas.Children.Add(_layerManager.GetLayer(_layerManager.GetLayerCount() - 1));
            _layerManager.GetLayer(_layerManager.GetLayerCount() - 1).DoubleTapped += Canvas_DoubleTapped;
            previewLayer = new Canvas();
            DrawingCanvas.Children.Add(previewLayer);

            UpdateCanvasSizes(ref _layerManager, ref DrawingCanvas);
        }

        /// <summary>
        /// Удаляет слой в объект класса отвечающий за работу со слоями
        /// </summary>
        /// <param name="_layerManager">Объект класса отвечающий за работу со слоями</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        /// <param name="currentLayerIndex">Индекс текущего слоя</param>
        /// <param name="previewLayer">Слой предпросмотра</param>
        /// <param name="defaultCanvasColor">Стандартный цвет слоя</param>
        /// <param name="Canvas_DoubleTapped">Обработчик события двойного нажатия</param>
        internal static void RemoveLayer(ref LayerManager? _layerManager, ref CustomCanvas? DrawingCanvas, ref int currentLayerIndex, ref Canvas? previewLayer, ref Color defaultCanvasColor, DoubleTappedEventHandler Canvas_DoubleTapped)
        {
            if (_layerManager == null || DrawingCanvas == null) return;

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
                AddLayer(ref _layerManager, ref DrawingCanvas, ref previewLayer, defaultCanvasColor, Canvas_DoubleTapped);
            }
        }

        /// <summary>
        /// Изначальная инициализация слоёв
        /// </summary>
        /// <param name="_layerManager">Объект класса отвечающий за работу со слоями</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        /// <param name="currentLayerIndex">Индекс текущего слоя</param>
        /// <param name="defaultCanvasColor">Стандартный цвет слоя</param>
        /// <param name="Canvas_DoubleTapped">Обработчик события двойного нажатия</param>
        internal static void InitializeLayers(ref int currentLayerIndex, ref LayerManager? _layerManager, ref CustomCanvas? DrawingCanvas, Color defaultCanvasColor, DoubleTappedEventHandler Canvas_DoubleTapped)
        {
            if (_layerManager == null || DrawingCanvas == null) return;

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
        /// Начальная инициализация слоя предпросмотра
        /// </summary>
        /// <param name="previewLayer">Слой предпросмотра</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        /// <param name="defaultCanvasColor">Стандартный цвет слоя</param>
        internal static void InitializePreviewLayer(ref Canvas? previewLayer, ref CustomCanvas? DrawingCanvas, Color defaultCanvasColor)
        {
            if (DrawingCanvas == null) return;

            previewLayer = new Canvas();
            previewLayer.Background = new SolidColorBrush(defaultCanvasColor);
            DrawingCanvas.Children.Add(previewLayer);
        }

        /// <summary>
        /// Уничтожение слоя предпросмотра с общего холста
        /// </summary>
        /// <param name="previewLayer">Слой предпросмотра</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        internal static void DestroyPreviewLayer(ref Canvas? previewLayer, ref CustomCanvas? DrawingCanvas)
        {
            if (DrawingCanvas == null) return;

            if (previewLayer != null)
            {
                DrawingCanvas.Children.Remove(previewLayer);
            }
            previewLayer = null;
        }

        /// <summary>
        /// Назначает активный слой
        /// </summary>
        /// <param name="currentLayerIndex">Индекс текущего слоя</param>
        /// <param name="index">Индекс назначаемого активным слоя</param>
        /// <param name="_layerManager">Объект класса отвечающий за работу со слоями</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        internal static void SetActiveLayer(int currentLayerIndex, ref LayerManager? _layerManager, ref CustomCanvas? DrawingCanvas, ref Canvas? currentLayer)
        {
            if (_layerManager == null || DrawingCanvas == null) return;

            currentLayer = _layerManager.GetLayer(currentLayerIndex);
            Canvas activeLayer = _layerManager.GetLayer(currentLayerIndex);

            DrawingCanvas.Children.Remove(activeLayer);
            DrawingCanvas.Children.Add(activeLayer);
        }

        /// <summary>
        /// Возвращает порядок слоёв
        /// </summary>
        /// <param name="_layerManager">Объект класса отвечающий за работу со слоями</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        internal static void ResetLayerOrder(ref LayerManager? _layerManager, ref CustomCanvas? DrawingCanvas)
        {
            if (_layerManager == null || DrawingCanvas == null) return;

            List<CustomCanvas> layers = _layerManager.GetAllLayers().ToList();
            DrawingCanvas.Children.Clear();

            foreach (var layer in layers)
            {
                DrawingCanvas.Children.Add(layer);
            }
        }

        /// <summary>
        /// Обновляет размеры слоёв в соответствии с холстом
        /// </summary>
        /// <param name="_layerManager">Объект класса отвечающий за работу со слоями</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        internal static void UpdateCanvasSizes(ref LayerManager? _layerManager, ref CustomCanvas? DrawingCanvas)
        {
            if (_layerManager == null || DrawingCanvas == null) return;

            foreach (Canvas layer in _layerManager.GetAllLayers())
            {
                layer.Width = DrawingCanvas.ActualWidth;
                layer.Height = DrawingCanvas.ActualHeight;
            }
        }

    }
}
