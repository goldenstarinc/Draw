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

            if (shape != null)
            {
                double left = Canvas.GetLeft(shape);
                double top = Canvas.GetTop(shape);
                double width = shape.Width;
                double height = shape.Height;

                if (shape.RenderTransform is RotateTransform transform)
                {
                    rotationAngle = transform.Angle;
                }

                selectionRectangle = new Rectangle
                {
                    Width = width + 8,
                    Height = height + 8,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(Color.FromArgb(50, 57, 97, 255)),
                    RenderTransform = new RotateTransform
                    {
                        Angle = rotationAngle,
                        CenterX = width / 2,
                        CenterY = height / 2
                    }
                };

                Canvas.SetLeft(selectionRectangle, left - 4);
                Canvas.SetTop(selectionRectangle, top - 4);

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

            rotationHandle = new Image
            {
                Width = 20,
                Height = 20,
                Source = new BitmapImage(new Uri("ms-appx:///Assets/rotate-icon.png")),
                Tag = "RotationHandle"
            };

            Canvas.SetLeft(rotationHandle, Canvas.GetLeft(shape) + shape.Width / 2 - rotationHandle.Width / 2);
            Canvas.SetTop(rotationHandle, Canvas.GetTop(shape) + shape.Height / 2 - rotationHandle.Height / 2);

            // Добавляем "ручку" на канвас
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
