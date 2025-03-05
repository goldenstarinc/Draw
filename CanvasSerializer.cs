using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Windows.UI;
using Newtonsoft;
using GraphicsLibrary;

namespace App3
{
    /// <summary>
    /// Модуль для сериализации
    /// </summary>
    public static class CanvasSerializer
    {
        /// <summary>
        /// Функция для сериализации канваса
        /// </summary>
        /// <param name="canvas">Канвас</param>
        /// <returns>Canvas-DTO</returns>
        public static CanvasDto SerializeCanvas(CustomCanvas canvas)
        {
            var dto = new CanvasDto();

            foreach (UIElement child in canvas.Children)
            {
                if (child is Shape shape)
                {
                    var brush = shape.Fill as SolidColorBrush;
                    var color = brush?.Color.ToString();
                    var stroke = shape.Stroke as SolidColorBrush;
                    var strokeColor = stroke?.Color.ToString();

                    if (shape.Name != "")
                    {
                        CanvasChildDto childDto = new CanvasChildDto
                        {
                            Name = shape.Name,
                            Width = shape.Width,
                            Height = shape.Height,
                            Left = CustomCanvas.GetLeft(shape),
                            Top = CustomCanvas.GetTop(shape),
                            FillColor = color,
                            StrokeColor = strokeColor,
                            StrokeThickness = shape.StrokeThickness
                        };
                        dto.Children.Add(childDto);
                    }
                    else if (shape is Polyline line)
                    {
                        foreach (var point in line.Points)
                        {
                            CanvasChildDto childDto = new CanvasChildDto
                            {
                                Name = line.Name,
                                Left = point.X,
                                Top = point.Y,
                                StrokeColor = strokeColor,
                                StrokeThickness = line.StrokeThickness
                            };
                            dto.Children.Add(childDto);
                        }
                    }
                }
            }
            return dto;
        }

        /// <summary>
        /// Функция для сохранения канваса в JSON-формате
        /// </summary>
        /// <param name="layerManager">Список слоев</param>
        /// <param name="filePath">Путь к файлу</param>
        public static void SaveCanvasToJson(LayerManager layerManager, string filePath)
        {
            string json = "";
            foreach (CustomCanvas canvas in layerManager.GetAllLayers())
            {
                var dto = SerializeCanvas(canvas);
                json += Newtonsoft.Json.JsonConvert.SerializeObject(dto, Formatting.Indented);
                json += ";";
            }
            json = json.Remove(json.Length - 1);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Функция для десериализации канваса
        /// </summary>
        /// <param name="json">JSON-файл</param>
        /// <returns>Десериализованный канвас</returns>
        public static CustomCanvas DeserializeCanvas(string json)
        {
            var dto = JsonConvert.DeserializeObject<CanvasDto>(json);

            double previousX = 0;
            double previousY = 0;
            string previousName = " ";

            var canvas = new CustomCanvas();
            foreach (var childDto in dto.Children)
            {
                Figure child = null;
                byte aF = 0, rF = 0, gF = 0, bF = 0;

                if (childDto.Name != "Line" && childDto.Name != "")
                {
                    aF = Convert.ToByte(childDto.FillColor.Substring(1, 2), 16);
                    rF = Convert.ToByte(childDto.FillColor.Substring(3, 2), 16);
                    gF = Convert.ToByte(childDto.FillColor.Substring(5, 2), 16);
                    bF = Convert.ToByte(childDto.FillColor.Substring(7, 2), 16);
                }

                byte a = Convert.ToByte(childDto.StrokeColor.Substring(1, 2), 16);
                byte r = Convert.ToByte(childDto.StrokeColor.Substring(3, 2), 16);
                byte g = Convert.ToByte(childDto.StrokeColor.Substring(5, 2), 16);
                byte b = Convert.ToByte(childDto.StrokeColor.Substring(7, 2), 16);

                switch (childDto.Name.ToLower())
                {
                    case "rectangle":
                        child = new RectangleFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness);
                        break;
                    case "circle":
                        child = new CircleFigure(childDto.Left, childDto.Top, childDto.Width, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness);
                        break;
                    case "triangle":
                        child = new TriangleFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness);
                        break;
                    case "righttriangle":
                        child = new RightTriangleFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness);
                        break;
                    case "goldenstar":
                        child = new GoldenStarFigure(childDto.Left, childDto.Top, childDto.Width, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness);
                        break;
                    case "rhombus":
                        child = new RhombusFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness);
                        break;
                    case "line":
                        child = new LineFigure(childDto.Left, childDto.Top, childDto.Left + childDto.Width, childDto.Top + childDto.Height, new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness);
                        break;
                    case "":
                        if (previousName != "")
                        {
                            previousX = childDto.Left;
                            previousY = childDto.Top;
                        }
                        child = new LineFigure(previousX, previousY, childDto.Left, childDto.Top, new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness + 2);
                        previousY = childDto.Top;
                        previousX = childDto.Left;
                        break;
                }

                if (child != null)
                {
                    child.Draw(canvas);
                }
                previousName = childDto.Name;
            }

            return canvas;
        }

        /// <summary>
        /// Загрузка канваса из JSON-файла
        /// </summary>
        /// <param name="filePath">Путь к JSON-файлу</param>
        /// <returns>Десериализованный канвас</returns>
        public static List<CustomCanvas> LoadCanvasFromJson(string filePath)
        {
            List<CustomCanvas> canvasList = new List<CustomCanvas>();

            string[] jsons = File.ReadAllText(filePath).Split(';');

            foreach (string json in jsons)
            {
                canvasList.Add(DeserializeCanvas(json));
            }

            return canvasList;
        }
    }
    /// <summary>
    /// Класс, представляющий DTO-Canvas
    /// </summary>
    public class CanvasDto
    {
        public List<CanvasChildDto> Children { get; set; } = new List<CanvasChildDto>();
    }

    /// <summary>
    /// Класс, представляющий коллекцию объектов канваса
    /// </summary>
    public class CanvasChildDto
    {
        public string? Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public string? FillColor { get; set; }
        public string? StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
    }
}
