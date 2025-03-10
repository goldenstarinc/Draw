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
        internal static void DrawSelectionRectangle(Shape shape, ref Rectangle? selectionRectangle, ref Canvas? currentLayer)
        {
            RemoveSelectionRectangle(ref currentLayer, ref selectionRectangle);

            if (currentLayer == null) return;

            if (shape != null)
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

                currentLayer.Children.Add(selectionRectangle);
            }
        }

        /// <summary>
        /// Метод, снимающий выделение с фигуры
        /// </summary>
        /// <param name="selectionRectangle">Выделяющий прямоугольник</param>
        /// <param name="currentLayer">Текущий слой</param>
        /// <param name="selectedShape">Выбранная фигура</param>
        internal static void RemoveSelection(ref Rectangle? selectionRectangle, ref Canvas? currentLayer, ref Shape? selectedShape)
        {
            if (selectionRectangle != null)
            {
                RemoveSelectionRectangle(ref currentLayer, ref selectionRectangle);
            }
            SetSelectedElementToNull(ref selectedShape);
        }

        /// <summary>
        /// Метод, удаляющий прямоугольник, отвечающий за выделение фигуры
        /// </summary>
        /// <param name="currentLayer">Текущий слой</param>
        /// <param name="selectionRectangle">Выделяющий прямоугольник</param>
        internal static void RemoveSelectionRectangle(ref Canvas? currentLayer, ref Rectangle? selectionRectangle)
        {
            if (currentLayer == null) return;

            if (selectionRectangle != null)
            {
                currentLayer.Children.Remove(selectionRectangle);
            }
            selectionRectangle = null;
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
