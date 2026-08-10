using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public class ScreenManager : PersistentSingleton<ScreenManager>
    {
        private const int RootSortingOrder = 0;
        private const int OverlaySortingStep = 10;

        private readonly ScreenNavigationService _navigation = new();
        private readonly Dictionary<ScreenDefinition, IScreen> _screens = new();

        public bool CanGoBack => _navigation.CanGoBack;

        #region Unity
        protected override void Awake()
        {
            base.Awake();

            _navigation.ScreenOpened += OnScreenOpened;
            _navigation.ScreenClosed += OnScreenClosed;
        }

        protected override void OnDestroy()
        {
            _navigation.ScreenOpened -= OnScreenOpened;
            _navigation.ScreenClosed -= OnScreenClosed;

            base.OnDestroy();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;

            Back();
        }
        #endregion

        public void Register(IScreen screen)
        {
            ScreenDefinition definition = screen.Definition;

            if (_screens.TryGetValue(definition, out IScreen registered))
            {
                if (registered == screen) return;

                Debug.LogError($"Screen '{definition.name}' is already registered by " +
                               $"'{registered.Name}', so '{screen.Name}' was ignored.");
                return;
            }

            _screens.Add(definition, screen);
            _navigation.Register(definition);
            screen.SetVisible(false);

            if (screen.OpenOnRegister && definition.Layer == ScreenLayer.Root && _navigation.Root == null)
            {
                Open(definition);
                return;
            }

            RefreshSorting();
        }

        public void Open(ScreenDefinition definition)
        {
            if (definition == null)
            {
                Debug.LogError("Something asked for a screen without a ScreenDefinition.", this);
                return;
            }

            if (!_navigation.IsRegistered(definition))
            {
                Debug.LogError($"Screen '{definition.name}' is not registered by any loaded scene.", this);
                return;
            }

            _navigation.Open(definition);
        }

        public void Back()
        {
            _navigation.Back();
        }


        public void Unregister(IScreen screen)
        {
            ScreenDefinition definition = screen.Definition;
            if (definition == null) return;

            if (!_screens.TryGetValue(definition, out IScreen registered) || registered != screen) return;

            _screens.Remove(definition);
            _navigation.Unregister(definition);
            RefreshSorting();
        }

        private void OnScreenOpened(ScreenDefinition definition)
        {
            _screens[definition].SetVisible(true);
            RefreshSorting();
        }

        private void OnScreenClosed(ScreenDefinition definition)
        {
            _screens[definition].SetVisible(false);
            RefreshSorting();
        }

        private void RefreshSorting()
        {
            if (_navigation.Root != null && _screens.TryGetValue(_navigation.Root, out IScreen root))
            {
                root.SetSortingOrder(RootSortingOrder);
            }

            IReadOnlyList<ScreenDefinition> overlays = _navigation.Overlays;
            for (int i = 0; i < overlays.Count; i++)
            {
                if (_screens.TryGetValue(overlays[i], out IScreen overlay))
                {
                    overlay.SetSortingOrder(OverlaySortingStep * (i + 1));
                }
            }
        }
    }
}
