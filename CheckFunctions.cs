using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using static App3.MainWindow;

namespace App3
{
    /// <summary>
    /// Модуль, содержащий проверяющие методы
    /// </summary>
    internal static class CheckFunctions
    {
        /// <summary>
        /// Метод, проверяющий содержит ли выбранная фигура переданную точку
        /// </summary>
        /// <param name="point">Переданная точка</param>
        /// <param name="element">Переданный элемент</param>
        /// <returns>True - если выбранная фигура содержит переданную точку, иначе - false</returns>
        internal static bool IsPointInsideElement(Point point, UIElement element)
        {
            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);
            double width = (element as FrameworkElement)?.Width ?? 0;
            double height = (element as FrameworkElement)?.Height ?? 0;

            return (point.X >= left && point.X <= left + width &&
                    point.Y >= top && point.Y <= top + height);
        }

        /// <summary>
        /// Метод, проверяющий находится ли переданная точка на границе выделенной фигуры
        /// </summary>
        /// <param name="point">Переданная точка</param>
        /// <param name="shape">Выделенная фигура</param>
        /// <param name="direction">Направления изменения размера</param>
        /// <returns>True - если точка находится на границе выделенной фигуры, иначе - false</returns>
        internal static bool IsOnBorder(Point point, Shape shape, out string direction)
        {
            direction = "";
            double left = Canvas.GetLeft(shape);
            double top = Canvas.GetTop(shape);
            double right = left + shape.Width;
            double bottom = top + shape.Height;

            if (Math.Abs(point.X - left) < 8 && point.Y <= bottom && point.Y >= top) direction = "left";
            else if (Math.Abs(point.X - right) < 8 && point.Y <= bottom && point.Y >= top) direction = "right";
            else if (Math.Abs(point.Y - top) < 8 && point.X >= left && point.X <= right) direction = "top";
            else if (Math.Abs(point.Y - bottom) < 8 && point.X >= left && point.X <= right) direction = "bottom";

            return !string.IsNullOrEmpty(direction);
        }

        /// <summary>
        /// Проверка, является ли выбранный инструмент фигурой
        /// </summary>
        /// <param name="selectedTool">Выбранный инструмент</param>
        /// <returns>True - если выбранный инструмент является фигурой, иначе false</returns>
        internal static bool IsFigure(Tool selectedTool) => availableShapes.Contains(selectedTool);
    }
}
