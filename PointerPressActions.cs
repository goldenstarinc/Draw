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
using Microsoft.UI.Xaml;

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
                else if (e.OriginalSource is Shape shape)
                {
                    selectedColor = ((SolidColorBrush)shape.Fill).Color;
                    colorPicker.Color = selectedColor;
                }
            }
            else if (e.OriginalSource is TextBlock textBlock)
            {
                selectedColor = ((SolidColorBrush)textBlock.Foreground).Color;
                colorPicker.Color = selectedColor;
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

            if (selectedTool == Tool.Eraser)
            {
                currentStroke.Stroke = new SolidColorBrush(Colors.White);
            }

            currentStroke.Points.Add(point);

            currentLayer.Children.Add(currentStroke);
        }

        /// <summary>
        /// Функция, отвечающая за написание создание текстбокса
        /// </summary>
        /// <param name="currentPoint">Текущая позиция курсора</param>
        /// <param name="textBox">Текстбокс</param>
        /// <param name="currentLayer">Текущий слой</param>
        /// <param name="selectedColor">Выбранный цвет</param>
        internal static void TypeText(Point currentPoint, ref TextBox? textBox, ref Canvas? currentLayer, Color selectedColor)
        {
            if (currentLayer == null) return;

            textBox = new TextBox
            {
                PlaceholderText = "Введите текст...",
                Width = 200,
                Height = 50,
                FontSize = 25,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Segoe UI"),
                Foreground = new SolidColorBrush(selectedColor)
            };

            Canvas.SetLeft(textBox, currentPoint.X - textBox.Height / 2 + 5);
            Canvas.SetTop(textBox, currentPoint.Y - textBox.Height / 2);

            textBox.LostFocus += TextBox_LostFocus;
            textBox.KeyDown += TextBox_KeyDown;

            currentLayer.Children.Add(textBox);

            textBox.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Функция, срабатывающая при потере фокуса текстбоксом
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Аргументы события</param>
        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Parent is Canvas canvas)
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    var textBlock = new TextBlock
                    {
                        Text = textBox.Text,
                        FontSize = textBox.FontSize,
                        FontFamily = textBox.FontFamily,
                        Foreground = textBox.Foreground,
                        Margin = textBox.Margin
                    };

                    Canvas.SetLeft(textBlock, Canvas.GetLeft(textBox));
                    Canvas.SetTop(textBlock, Canvas.GetTop(textBox));

                    canvas.Children.Add(textBlock);
                }

                canvas.Children.Remove(textBox);
            }
        }

        /// <summary>
        /// Функция, срабатывающая во время нажатия кнопки при активном текстбоксе
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Аргументы события, содержащие информацию о нажатии клавиши</param>
        private static void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (sender is TextBox textBox && e.Key == Windows.System.VirtualKey.Enter)
            {
                TextBox_LostFocus(sender, e);
            }
        }
    }
}
