using System;
using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;

namespace Gazeus.DesafioMatch3.Core
{
    public class ScreenNavigationService
    {
        public event Action<ScreenDefinition> ScreenOpened;
        public event Action<ScreenDefinition> ScreenClosed;

        private readonly HashSet<ScreenDefinition> _registered = new();
        private readonly List<ScreenDefinition> _overlays = new();

        public ScreenDefinition Root { get; private set; }
        public IReadOnlyList<ScreenDefinition> Overlays => _overlays;
        public bool CanGoBack => _overlays.Count > 0;

        public bool IsRegistered(ScreenDefinition definition)
        {
            return definition != null && _registered.Contains(definition);
        }

        public bool Register(ScreenDefinition definition)
        {
            return definition != null && _registered.Add(definition);
        }

        public void Unregister(ScreenDefinition definition)
        {
            if (!_registered.Remove(definition)) return;

            _overlays.Remove(definition);

            if (Root == definition) Root = null;
        }

        public void Open(ScreenDefinition definition)
        {
            if (!IsRegistered(definition)) return;

            if (definition.Layer == ScreenLayer.Overlay) OpenOverlay(definition);
            else OpenRoot(definition);
        }

        public void Back()
        {
            if (_overlays.Count == 0) return;

            CloseOverlay(_overlays.Count - 1);
        }

        private void OpenRoot(ScreenDefinition definition)
        {
            if (Root == definition) return;

            for (int i = _overlays.Count - 1; i >= 0; i--)
            {
                CloseOverlay(i);
            }

            ScreenDefinition previous = Root;
            Root = definition;

            if (previous != null) ScreenClosed?.Invoke(previous);

            ScreenOpened?.Invoke(definition);
        }

        private void OpenOverlay(ScreenDefinition definition)
        {
            if (_overlays.Contains(definition)) return;

            _overlays.Add(definition);
            ScreenOpened?.Invoke(definition);
        }

        private void CloseOverlay(int index)
        {
            ScreenDefinition definition = _overlays[index];
            _overlays.RemoveAt(index);
            ScreenClosed?.Invoke(definition);
        }
    }
}
