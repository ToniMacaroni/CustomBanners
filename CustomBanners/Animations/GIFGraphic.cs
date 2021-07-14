using CustomBanners.Graphics;
using System.Collections.Generic;
using UnityEngine;

namespace CustomBanners.Animations
{
    internal class GIFGraphic : IGraphic
    {
        private readonly ISet<IGraphicListener> _listeners = new HashSet<IGraphicListener>();
        public bool HasListeners => _listeners.Count > 0;
        public Texture2D Graphic { get; set; }
        public Texture2D Default { get; }
        public string Name { get; }

        public GIFGraphic(string name, Texture2D baseTexture)
        {
            Name = name;
            Graphic = baseTexture;
            Default = Graphic;
        }

        public void AddListener(IGraphicListener listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(IGraphicListener listener)
        {
            _listeners.Remove(listener);
        }

        public void SetGraphic(Texture2D texture)
        {
            Graphic = texture;
            foreach (var listener in _listeners)
                listener.UpdateTexture(Graphic);
        }
    }
}