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
            bool newBrush = true;

            foreach (UIElement child in canvas.Children)
            {
                if (child is Shape shape && shape.Tag is Figure figure)
                {
                    // get fill color
                    SolidColorBrush? fillColorBrush = figure.FillColor as SolidColorBrush;
                    fillColorBrush = fillColorBrush ?? new SolidColorBrush(Colors.White);
                    string fillColor = fillColorBrush.Color.ToString();

                    // get stroke color
                    SolidColorBrush? strokeColorBrush = figure.StrokeColor as SolidColorBrush;
                    strokeColorBrush = strokeColorBrush ?? new SolidColorBrush(Colors.Black);
                    string strokeColor = strokeColorBrush.Color.ToString();

                    // get rotation angle
                    double rotationAngle = figure.RotationAngle;

                    if (figure is not LineFigure)
                    {
                        CanvasChildDto childDto = new CanvasChildDto
                        {
                            Name = shape.Name,
                            Width = shape.Width,
                            Height = shape.Height,
                            Left = CustomCanvas.GetLeft(shape),
                            Top = CustomCanvas.GetTop(shape),
                            FillColor = fillColor,
                            StrokeColor = strokeColor,
                            StrokeThickness = shape.StrokeThickness,
                            RotationAngle = rotationAngle
                        };

                        dto.Children.Add(childDto);
                    }
                    else if (figure is LineFigure line)
                    {
                        CanvasChildDto childDto = new CanvasChildDto
                        {
                            Name = shape.Name,
                            Width = line.X2,
                            Height = line.Y2,
                            Left = line.X,
                            Top = line.Y,
                            FillColor = fillColor,
                            StrokeColor = strokeColor,
                            StrokeThickness = shape.StrokeThickness,
                            RotationAngle = rotationAngle
                        };

                        dto.Children.Add(childDto);
                    }
                }
                else if (child is Shape currentShape && currentShape != null && currentShape is Polyline line)
                {
                    // get stroke color
                    SolidColorBrush? strokeColorBrush = currentShape.Stroke as SolidColorBrush;
                    strokeColorBrush = strokeColorBrush ?? new SolidColorBrush(Colors.Black);
                    string strokeColor = strokeColorBrush.Color.ToString();


                    foreach (var point in line.Points)
                    {
                        CanvasChildDto childDto = new CanvasChildDto
                        {
                            Name = line.Name,
                            Left = point.X,
                            Top = point.Y,
                            Width = !newBrush ? dto.Children.Last().Left : point.X,
                            Height = !newBrush ? dto.Children.Last().Top : point.Y,
                            FillColor = Colors.Black.ToString(),
                            StrokeColor = strokeColor,
                            StrokeThickness = line.StrokeThickness,
                            RotationAngle = 0
                        };
                        if(newBrush)
                            newBrush = false;
                        dto.Children.Add(childDto);
                    }
                }
                newBrush = true;
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
                CanvasDto dto = SerializeCanvas(canvas);
                json += JsonConvert.SerializeObject(dto, Formatting.Indented);
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
        public static CustomCanvas? DeserializeCanvas(string json)
        {
            CanvasDto? canvasDto = JsonConvert.DeserializeObject<CanvasDto>(json);

            if (canvasDto == null) return null;

            CustomCanvas canvas = new CustomCanvas();

            foreach (CanvasChildDto childDto in canvasDto.Children)
            {
                Figure? figure = null;

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
                        figure = new RectangleFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness, childDto.RotationAngle);
                        break;
                    case "circle":
                        figure = new CircleFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness, childDto.RotationAngle);
                        break;
                    case "triangle":
                        figure = new TriangleFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness, childDto.RotationAngle);
                        break;
                    case "righttriangle":
                        figure = new RightTriangleFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness, childDto.RotationAngle);
                        break;
                    case "goldenstar":
                        figure = new GoldenStarFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness, childDto.RotationAngle);
                        break;
                    case "rhombus":
                        figure = new RhombusFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(aF, rF, gF, bF)), new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness, childDto.RotationAngle);
                        break;
                    case "line":
                        figure = new LineFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness, childDto.RotationAngle);
                        break;
                    case "":
                        figure = new LineFigure(childDto.Left, childDto.Top, childDto.Width, childDto.Height, new SolidColorBrush(Color.FromArgb(a, r, g, b)), childDto.StrokeThickness, 0);
                        break;
                }

                if (figure != null) figure.Draw(canvas);
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
        public required string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public required string FillColor { get; set; }
        public required string StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        public double RotationAngle { get; set; }
    }
}
