using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using GraphicsLibrary;

namespace App3
{
    
    /// <summary>
    /// Модуль, содержащий функции для работы с выделением объектов
    /// </summary>
    internal static class SelectionFunctions
    {
        /// <summary>
        /// Метод, выделяющий выбранную фигуру
        /// </summary>
        /// <param name="shape">Фигура</param>
        /// <param name="selectionRectangle">Выделяющий прямоугольник</param>
        /// <param name="currentLayer">Текущий слой</param>
        /// <param name="rotationHandle">Ручка поворота фигуры</param>
        internal static void DrawSelectionRectangle(Shape shape, ref Rectangle? selectionRectangle, ref Canvas? currentLayer, ref Image? rotationHandle, ref double rotationAngle)
        {
            RemoveSelectionRectangle(ref currentLayer, ref selectionRectangle, ref rotationHandle);

            if (currentLayer == null) return;

            if (shape != null && shape.Tag is Figure figure)
            {
                double left = figure.X;
                double top = figure.Y;
                double right = figure.X + figure.Width;
                double bottom = figure.Y + figure.Height;

                rotationAngle = (shape.RenderTransform is RotateTransform transform) ? transform.Angle : 0;

                double offset = 4;
                selectionRectangle = new Rectangle
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(Color.FromArgb(50, 57, 97, 255))
                };

                if (figure is LineFigure line)
                {
                    double minX = Math.Min(line.X, line.X2);
                    double minY = Math.Min(line.Y, line.Y2);
                    double maxX = Math.Max(line.X, line.X2);
                    double maxY = Math.Max(line.Y, line.Y2);

                    double width = maxX - minX + offset * 2;
                    double height = maxY - minY + offset * 2;

                    Canvas.SetLeft(selectionRectangle, minX - offset);
                    Canvas.SetTop(selectionRectangle, minY - offset);

                    selectionRectangle.Width = width;
                    selectionRectangle.Height = height;

                    selectionRectangle.RenderTransform = new RotateTransform
                    {
                        Angle = rotationAngle,
                        CenterX = (width) / 2,
                        CenterY = (height) / 2
                    };
                }
                else
                {
                    Canvas.SetLeft(selectionRectangle, left - offset);
                    Canvas.SetTop(selectionRectangle, top - offset);
                    selectionRectangle.Width = shape.Width + offset * 2;
                    selectionRectangle.Height = shape.Height + offset * 2;
                    selectionRectangle.RenderTransform = new RotateTransform
                    {
                        Angle = rotationAngle,
                        CenterX = (shape.Width + offset * 2) / 2,
                        CenterY = (shape.Height + offset * 2) / 2
                    };
                }

                currentLayer.Children.Add(selectionRectangle);
                CreateRotationHandle(shape, ref currentLayer, ref rotationHandle);
            }
        }

        /// <summary>
        /// Метод, снимающий выделение с фигуры
        /// </summary>
        /// <param name="selectionRectangle">Выделяющий прямоугольник</param>
        /// <param name="currentLayer">Текущий слой</param>
        /// <param name="selectedShape">Выбранная фигура</param>
        /// <param name="rotationHandle">Ручка поворота фигуры</param>
        internal static void RemoveSelection(ref Rectangle? selectionRectangle, ref Canvas? currentLayer, ref Shape? selectedShape, ref Image? rotationHandle)
        {
            if (selectionRectangle != null)
            {
                RemoveSelectionRectangle(ref currentLayer, ref selectionRectangle, ref rotationHandle);
            }
            SetSelectedElementToNull(ref selectedShape);
        }

        /// <summary>
        /// Метод, удаляющий прямоугольник, отвечающий за выделение фигуры
        /// </summary>
        /// <param name="currentLayer">Текущий слой</param>
        /// <param name="selectionRectangle">Выделяющий прямоугольник</param>
        /// <param name="rotationHandle">Ручка поворота фигуры</param>
        internal static void RemoveSelectionRectangle(ref Canvas? currentLayer, ref Rectangle? selectionRectangle, ref Image? rotationHandle)
        {
            RemoveRotationHandle(ref currentLayer, ref rotationHandle);

            if (currentLayer == null) return;

            if (selectionRectangle != null)
            {
                currentLayer.Children.Remove(selectionRectangle);
            }
            selectionRectangle = null;
        }

        /// <summary>
        /// Функция, создающая ручку для поворота фигуры
        /// </summary>
        /// <param name="shape">Фигура</param>
        /// <param name="currentLayer">Текущий слой</param>
        /// <param name="rotationHandle">Ручка поворота</param>
        internal static void CreateRotationHandle(Shape shape, ref Canvas currentLayer, ref Image? rotationHandle)
        {
            if (shape == null) return;

            // Создаём новую ручку вращения
            rotationHandle = new Image
            {
                Width = 20,
                Height = 20,
                Source = new BitmapImage(new Uri("ms-appx:///Assets/rotate-icon.png")),
                Tag = "RotationHandle"
            };

            if (shape.Tag is LineFigure line)
            {
                double centerX = (line.X + line.X2) / 2;
                double centerY = (line.Y + line.Y2) / 2;

                Canvas.SetLeft(rotationHandle, centerX - rotationHandle.Width / 2);
                Canvas.SetTop(rotationHandle, centerY - rotationHandle.Height / 2);
            }
            else
            {
                Canvas.SetLeft(rotationHandle, Canvas.GetLeft(shape) + shape.Width / 2 - rotationHandle.Width / 2);
                Canvas.SetTop(rotationHandle, Canvas.GetTop(shape) + shape.Height / 2 - rotationHandle.Height / 2);
            }

            currentLayer.Children.Add(rotationHandle);
        }

        /// <summary>
        /// Функция, удаляющая ручку поворота фигуры
        /// </summary>
        /// <param name="currentLayer">Текущий слой</param>
        /// <param name="rotationHandle">Ручка поворота фигуры</param>
        internal static void RemoveRotationHandle(ref Canvas? currentLayer, ref Image? rotationHandle)
        {
            if (currentLayer == null) return;

            if (rotationHandle != null)
            {
                currentLayer.Children.Remove(rotationHandle);
            }
        }

        /// <summary>
        /// Метод, удаляющий выбранную фигуру с текущего слоя
        /// </summary>
        /// <param name="selectedShape">Выделяющий прямоугольник</param>
        /// <param name="currentLayer">Текущий слой</param>
        internal static void RemoveSelectedShapeFromLayer(Shape? selectedShape, ref Canvas? currentLayer)
        {
            if (currentLayer == null) return;

            if (selectedShape != null)
            {
                currentLayer.Children.Remove(selectedShape);
            }
        }

        /// <summary>
        /// Метод, зануляющий выбранную фигуру
        /// </summary>
        /// <param name="selectedShape">Выбранная фигура</param>
        internal static void SetSelectedElementToNull(ref Shape? selectedShape) => selectedShape = null;
    }
}
