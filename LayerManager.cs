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
    public class LayerManager
    {
        private List<CustomCanvas> _layers = new List<CustomCanvas>();

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

        public void RemoveLayer(int index)
        {
            if (index >= 0 && index < _layers.Count)
            {
                _layers.RemoveAt(index);
            }
        }

        public void BringToFront(int index)
        {
            if (index >= 0 && index < _layers.Count)
            {
                var layer = _layers[index];
                _layers.Remove(layer);
                _layers.Add(layer);
            }
        }

        public void SendToBack(int index)
        {
            if (index >= 0 && index < _layers.Count)
            {
                var layer = _layers[index];
                _layers.Remove(layer);
                _layers.Insert(0, layer);
            }
        }

        public Canvas GetLayer(int index)
        {
            if (index == -1) throw new Exception("Нет слоёв!");
            return _layers[index];
        }

        public int GetLayerCount()
        {
            return _layers.Count;
        }

        public void ClearAllLayers()
        {
            _layers.Clear();
        }

        public IEnumerable<CustomCanvas> GetAllLayers()
        {
            return _layers;
        }
    }
}