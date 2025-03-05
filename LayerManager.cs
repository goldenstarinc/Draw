using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;

namespace App3
{
    /// <summary>
    /// Класс для работы со слоями
    /// </summary>
    public class LayerManager
    {
        private List<CustomCanvas> _layers = new List<CustomCanvas>();

        /// <summary>
        /// Добавление слоя
        /// </summary>
        /// <param name="canvas">Слой</param>
        public void AddLayer(CustomCanvas canvas = null)
        {
            if (canvas != null)
            {
                _layers.Add(canvas);
            }
            else
            {
                var newLayer = new CustomCanvas();
                _layers.Add(newLayer);
            }
        }

        /// <summary>
        /// Удаление слоя
        /// </summary>
        /// <param name="index">Индекс для удаления</param>
        public void RemoveLayer(int index)
        {
            if (index >= 0 && index < _layers.Count)
            {
                _layers.RemoveAt(index);
            }
        }

        /// <summary>
        /// Вынести слой на передний план
        /// </summary>
        /// <param name="index">Индекс слоя</param>
        public void BringToFront(int index)
        {
            if (index >= 0 && index < _layers.Count)
            {
                var layer = _layers[index];
                _layers.Remove(layer);
                _layers.Add(layer);
            }
        }

        /// <summary>
        /// Вынести слой на задний план
        /// </summary>
        /// <param name="index">Индекс слоя</param>
        public void SendToBack(int index)
        {
            if (index >= 0 && index < _layers.Count)
            {
                var layer = _layers[index];
                _layers.Remove(layer);
                _layers.Insert(0, layer);
            }
        }

        /// <summary>
        /// Получить слой по индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <returns>Слой</returns>
        public Canvas GetLayer(int index)
        {
            if (index == -1) return null;
            return _layers[index];
        }

        /// <summary>
        /// Получить количество слоев
        /// </summary>
        /// <returns>ВКоличество слоев</returns>
        public int GetLayerCount()
        {
            return _layers.Count;
        }

        /// <summary>
        /// Очистить слои
        /// </summary>
        public void ClearAllLayers()
        {
            _layers.Clear();
        }

        /// <summary>
        /// Получить все слои
        /// </summary>
        /// <returns>Слои</returns>
        public IEnumerable<CustomCanvas> GetAllLayers()
        {
            return _layers;
        }
    }
}