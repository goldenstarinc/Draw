using Microsoft.UI.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App3
{
    /// <summary>
    /// Модуль, содержащий функции для отслеживания и изменения стиля курсора
    /// </summary>
    internal static class CursorManager
    {
        /// <summary>
        /// Enum, содержащий возможные стили курсора
        /// </summary>
        internal enum CursorStates
        {
            Default,
            Drawing,
            Selecting,
            SizeNorthwestSoutheast,
            SizeNortheastSouthwest,
            SizeWestEast,
            SizeNorthSouth
        }

        /// <summary>
        /// Изменение стиля курсора в зависимости от состояния
        /// </summary>
        /// <param name="state">Состояние</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        internal static void ChangeCursor(CursorStates state, ref CustomCanvas? DrawingCanvas)
        {
            if (DrawingCanvas == null) return;

            switch (state)
            {
                case CursorStates.Default:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                    break;
                case CursorStates.Drawing:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.Cross);
                    break;
                case CursorStates.Selecting:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
                    break;
                case CursorStates.SizeNorthwestSoutheast:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthwestSoutheast);
                    break;
                case CursorStates.SizeNortheastSouthwest:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNortheastSouthwest);
                    break;
                case CursorStates.SizeWestEast:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
                    break;
                case CursorStates.SizeNorthSouth:
                    DrawingCanvas.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthSouth);
                    break;
            }
        }

        /// <summary>
        /// Изменение стиля курсора в зависимости от направления изменения размера фигуры
        /// </summary>
        /// <param name="direction">Направление изменения размера фигуры</param>
        /// <param name="DrawingCanvas">Общий холст</param>
        internal static void UpdateCursorForResizeDirection(string direction, ref CustomCanvas? DrawingCanvas)
        {
            if (DrawingCanvas == null) return;

            if (string.IsNullOrEmpty(direction)) return;

            switch (direction)
            {
                case "right":
                    ChangeCursor(CursorStates.SizeWestEast, ref DrawingCanvas);
                    break;
                case "left":
                    ChangeCursor(CursorStates.SizeWestEast, ref DrawingCanvas);
                    break;
                case "bottom":
                    ChangeCursor(CursorStates.SizeNorthSouth, ref DrawingCanvas);
                    break;
                case "top":
                    ChangeCursor(CursorStates.SizeNorthSouth, ref DrawingCanvas);
                    break;
                case "bottom-right":
                    ChangeCursor(CursorStates.SizeNorthwestSoutheast, ref DrawingCanvas);
                    break;
                case "top-left":
                    ChangeCursor(CursorStates.SizeNorthwestSoutheast, ref DrawingCanvas);
                    break;
                case "top-right":
                    ChangeCursor(CursorStates.SizeNortheastSouthwest, ref DrawingCanvas);
                    break;
                case "bottom-left":
                    ChangeCursor(CursorStates.SizeNortheastSouthwest, ref DrawingCanvas);
                    break;
                default:
                    ChangeCursor(CursorStates.Default, ref DrawingCanvas);
                    break;
            }
        }
    }
}
