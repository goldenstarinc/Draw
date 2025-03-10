using GraphicsLibrary;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

using static App3.MainWindow;
using static App3.SelectionFunctions;

namespace App3
{
    // <summary>
    /// Модуль, содержащий функции работы с объектами при движении курсора
    /// </summary>
    internal static class PointerMoveActions
    {
        /// <summary>
        /// Функция, отвечающая за рисование кистью
        /// </summary>
        /// <param name="currentPoint">Текщее положение курсора</param>
        /// <param name="currentStroke">Текущая последовательность точек</param>
        internal static void Draw(Point currentPoint, ref Polyline? currentStroke)
        {
            if (currentStroke == null) return;

            currentStroke.Points.Add(currentPoint);
        }

        /// <summary>
        /// Функция для отрисовки выбранной фигуры
        /// </summary>
        /// <param name="currentPoint">Текщее положение курсора</param>
        /// <param name="startPoint">Начальное положение курсора</param>
        /// <param name="selectedTool">Выбранный инструмент</param>
        /// <param name="defaultCanvasColor">Стандартный цвет канваса (прозрачный)</param>
        /// <param name="selectedColor">Выбранный цвет</param>
        /// <param name="previewFigure">Нарисованная фигура</param>
        /// <param name="previewLayer">Слой для рисования</param>
        internal static void DrawFigure(Point currentPoint, Point startPoint, Tool selectedTool, Color defaultCanvasColor, Color selectedColor, ref Figure? previewFigure, ref Canvas? previewLayer)
        {
            if (previewLayer == null) return; 

            double x = Math.Min(startPoint.X, currentPoint.X);
            double y = Math.Min(startPoint.Y, currentPoint.Y);
            double width = Math.Abs(currentPoint.X - startPoint.X);
            double height = Math.Abs(currentPoint.Y - startPoint.Y);

            if (selectedTool == Tool.Rectangle)
            {
                previewFigure = new RectangleFigure(x, y, width, height, new SolidColorBrush(defaultCanvasColor), new SolidColorBrush(selectedColor), 2);
            }
            else if (selectedTool == Tool.Circle)
            {
                previewFigure = new CircleFigure(x, y, width, height, new SolidColorBrush(defaultCanvasColor), new SolidColorBrush(selectedColor), 2);
            }
            else if (selectedTool == Tool.Line)
            {
                previewFigure = new LineFigure(startPoint.X, startPoint.Y, currentPoint.X, currentPoint.Y, new SolidColorBrush(selectedColor), 2);
            }
            else if (selectedTool == Tool.Triangle)
            {
                previewFigure = new TriangleFigure(x, y, width, height, new SolidColorBrush(defaultCanvasColor), new SolidColorBrush(selectedColor), 2);
            }
            else if (selectedTool == Tool.RightTriangle)
            {
                previewFigure = new RightTriangleFigure(x, y, width, height, new SolidColorBrush(defaultCanvasColor), new SolidColorBrush(selectedColor), 2);
            }
            else if (selectedTool == Tool.Rhombus)
            {
                previewFigure = new RhombusFigure(x, y, width, height, new SolidColorBrush(defaultCanvasColor), new SolidColorBrush(selectedColor), 2);
            }
            else if (selectedTool == Tool.GoldenStar)
            {
                previewFigure = new GoldenStarFigure(x, y, width / 2, height / 2, new SolidColorBrush(defaultCanvasColor), new SolidColorBrush(selectedColor), 2);
            }
            else if (selectedTool == Tool.Person)
            {
                previewFigure = new PersonFigure(x, y, width, height, new SolidColorBrush(defaultCanvasColor), new SolidColorBrush(selectedColor), 2);
            }

            previewLayer.Children.Clear();
            if (previewFigure != null) previewFigure.Draw(previewLayer);
        }

        /// <summary>
        /// Функция для перемещения выбранной фигуры
        /// </summary>
        /// <param name="currentPoint">Текщее положение курсора</param>
        /// <param name="startPoint">Начальное положение курсора</param>
        /// <param name="selectedShape">Выбранная фигура</param>
        /// <param name="previewLayer">Слой предпросмотра</param>
        /// <param name="currentLayer">Текущий слой</param>
        /// <param name="previewFigure">Фигура предпросмотра</param>
        /// <param name="selectionRectangle">Выделяющий прямоугольник</param>
        internal static void Drag(Point currentPoint, Point startPoint, ref Shape? selectedShape, ref Canvas? previewLayer, ref Canvas? currentLayer, ref Figure? previewFigure, ref Rectangle? selectionRectangle)
        {
            if (previewLayer == null || currentLayer == null) return;

            if (selectedShape != null)
            {
                // line
                if (selectedShape.Tag is LineFigure line)
                {
                    double dx = currentPoint.X - startPoint.X;
                    double dy = currentPoint.Y - startPoint.Y;

                    line.X += dx;
                    line.Y += dy;
                    line.X2 += dx;
                    line.Y2 += dy;

                    previewLayer.Children.Clear();
                    line.Draw(previewLayer);

                    previewFigure = line;
                }
                // other figures
                else if (selectedShape.Tag is Figure figure)
                {
                    figure.X = currentPoint.X - figure.Width / 2;
                    figure.Y = currentPoint.Y - figure.Height / 2;
                    figure.FillColor = selectedShape.Fill;
                    figure.StrokeColor = selectedShape.Stroke;

                    previewLayer.Children.Clear();
                    figure.Draw(previewLayer);

                    previewFigure = figure;
                }

                currentLayer.Children.Remove(selectedShape);

                RemoveSelectionRectangle(ref currentLayer, ref selectionRectangle);

                startPoint = currentPoint;

                if (previewLayer.Children.Count > 0)
                {
                    selectedShape = previewLayer.Children.Last() as Shape;
                }
            }
        }

        /// <summary>
        /// Функция, отвечающая за изменение размера фигуры
        /// </summary>
        /// <param name="currentPoint">Текщее положение курсора</param>
        /// <param name="startPoint">Начальное положение курсора</param>
        /// <param name="selectedShape">Выброанная фигура</param>
        /// <param name="resizeDirection">Направление изменения размера</param>
        /// <param name="selectedTool">Выбранный инструмент</param>
        /// <param name="previewFigure">Фигура</param>
        /// <param name="selectionRectangle">Выделяющий прямоугольник</param>
        /// <param name="previewLayer">Слой предпросмотра</param>
        /// <param name="currentLayer">Текущий слой</param>
        internal static void Resize(Point currentPoint, Point startPoint, ref Shape? selectedShape, string resizeDirection, Tool selectedTool, ref Figure? previewFigure, ref Rectangle? selectionRectangle, ref Canvas? previewLayer, ref Canvas? currentLayer)
        {
            if (previewLayer == null || currentLayer == null) return;

            if (selectedShape != null && selectedShape.Tag is Figure figure)
            {
                double x = figure.X;
                double y = figure.Y;
                double width = figure.Width;
                double height = figure.Height;

                SolidColorBrush fillColorBrush = (SolidColorBrush)selectedShape.Fill;
                SolidColorBrush strokeColorBrush = (SolidColorBrush)selectedShape.Stroke;

                if (resizeDirection == "right")
                {
                    double dx = currentPoint.X - (x + width);

                    if (width + dx > 0)
                    {
                        width += dx;
                    }
                }
                else if (resizeDirection == "left")
                {
                    double dx = x - currentPoint.X;

                    if (width + dx > 0)
                    {
                        width += dx;
                    }

                    x -= dx;
                }
                else if (resizeDirection == "top")
                {
                    double dy = y - currentPoint.Y;

                    if (height + dy > 0)
                    {
                        height += dy;
                    }

                    y -= dy;
                }
                else if (resizeDirection == "bottom")
                {
                    double dy = (currentPoint.Y - height) - y;

                    if (height + dy > 0)
                    {
                        height += dy;
                    }
                }

                if (selectedTool == Tool.Rectangle)
                {
                    previewFigure = new RectangleFigure(x, y, width, height, fillColorBrush, strokeColorBrush, 2);
                }
                else if (figure is CircleFigure)
                {
                    previewFigure = new CircleFigure(x, y, width, height, fillColorBrush, strokeColorBrush, 2);
                }
                else if (selectedTool == Tool.Line)
                {
                    previewFigure = new LineFigure(startPoint.X, startPoint.Y, currentPoint.X, currentPoint.Y, fillColorBrush, 2);
                }
                else if (selectedTool == Tool.Triangle)
                {
                    previewFigure = new TriangleFigure(x, y, width, height, fillColorBrush, strokeColorBrush, 2);
                }
                else if (selectedTool == Tool.RightTriangle)
                {
                    previewFigure = new RightTriangleFigure(x, y, width, height, fillColorBrush, strokeColorBrush, 2);
                }
                else if (selectedTool == Tool.Rhombus)
                {
                    previewFigure = new RhombusFigure(x, y, width, height, fillColorBrush, strokeColorBrush, 2);
                }
                else if (selectedTool == Tool.GoldenStar)
                {
                    previewFigure = new GoldenStarFigure(x, y, width, height, fillColorBrush, strokeColorBrush, 2);
                }
                else if (selectedTool == Tool.Person)
                {
                    previewFigure = new PersonFigure(x, y, width, height, fillColorBrush, strokeColorBrush, 2);
                }

                RemoveSelectedShapeFromLayer(selectedShape, ref currentLayer);
                RemoveSelectionRectangle(ref currentLayer, ref selectionRectangle);

                previewLayer.Children.Clear();

                if (previewFigure != null) previewFigure.Draw(previewLayer);

                if (previewLayer.Children.Count > 0)
                {
                    selectedShape = previewLayer.Children.Last() as Shape;
                }
            }
        }
    }
}
