using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App3.MainWindow;
using Windows.UI;
using Windows.Foundation;
using GraphicsLibrary;

namespace App3
{
    /// <summary>
    /// Модуль, содержащий функции работы с объектами при одиночном нажатии
    /// </summary>
    internal static class PointerPressActions
    {
        /// <summary>
        /// Заливает объект выбранным цветом
        /// </summary>
        /// <param name="e">Аргументы, переданные курсором</param>
        /// <param name="selectedColor">Выбранный цвет</param>
        internal static void FillObject(ref PointerRoutedEventArgs e, SolidColorBrush selectedColor)
        {
            if (e.OriginalSource is Shape selectedShape && selectedShape.Tag is Figure figure)
            {
                selectedShape.Fill = selectedColor;
                figure.FillColor = selectedColor;
            }
            else if (e.OriginalSource is Canvas selectedCanvas)
            {
                selectedCanvas.Background = selectedColor;
            }
        }

        /// <summary>
        /// Метод, копирующий цвет объекта
        /// </summary>
        /// <param name="e">Аргументы, переданные курсорам</param>
        /// <param name="colorPicker">Палитра</param>
        internal static void PickColor(ref PointerRoutedEventArgs e, ref ColorPicker colorPicker, ref Color selectedColor)
        {
            if (e.OriginalSource is Shape selectedShape)
            {
                if (selectedShape is Polyline line)
                {
                    selectedColor = ((SolidColorBrush)line.Stroke).Color;
                    colorPicker.Color = selectedColor;
                }
                else
                {
                    selectedColor = ((SolidColorBrush)selectedShape.Fill).Color;
                    colorPicker.Color = selectedColor;
                }
            }
            else if (e.OriginalSource is Canvas selectedCanvas)
            {
                selectedColor = ((SolidColorBrush)selectedCanvas.Background).Color;
                colorPicker.Color = selectedColor;
            }
        }

        /// <summary>
        /// Функция, отвечающая за начало рисования / стирание
        /// </summary>
        /// <param name="point">Текущее положение курсора</param>
        /// <param name="currentStroke">Текущая последовательность точек</param>
        /// <param name="selectedColor">Выбранный цвет</param>
        /// <param name="selectedStrokeThickness">Выбранная толщина</param>
        /// <param name="selectedTool">Выбранный инструмент</param>
        /// <param name="currentLayer">Текущий слой</param>
        internal static void StartDrawing(Point point, ref Polyline? currentStroke, Color selectedColor, double selectedStrokeThickness, Tool selectedTool, ref Canvas? currentLayer)
        {
            if (currentLayer == null) return;

            currentStroke = new Polyline
            {
                Stroke = new SolidColorBrush(selectedColor),
                StrokeThickness = selectedStrokeThickness
            };

            if (selectedTool == Tool.Eraser) // ?????????????????????
            {
                currentStroke.Stroke = new SolidColorBrush(Colors.White);
            }

            currentStroke.Points.Add(point);

            currentLayer.Children.Add(currentStroke);
        }
    }
}
