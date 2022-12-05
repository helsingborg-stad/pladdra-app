using System;
using System.Linq;
using UnityEngine;

namespace Screens
{

    public class ScreenManager: MonoBehaviour{
        public ScreenManager()
        {
            _screens = new Screen[0];
        }

        [SerializeField]
        private Screen _selectedScreen;

        [SerializeField]
        private Screen[] _screens;

        public void SetActiveScreen<TScreen>(
                Action<TScreen> beforeActivate = null,
                Action<TScreen> afterActivate = null
            ) where TScreen: Screen {

            _screens
                .OfType<TScreen>()
                .Take(1)
                .Select(screen => {
                    _selectedScreen?.SetScreenActive(false);
                    _selectedScreen = screen;
                    beforeActivate?.Invoke(screen as TScreen);
                    _selectedScreen.SetScreenActive(true);
                    afterActivate?.Invoke(screen as TScreen);
                    return 0;
                })
                .ToArray();
        }

        private void Awake() {
            foreach(var screen in _screens) {
                screen.SetScreenActive(false);
            }
        }


        private void Start() {
            if (_selectedScreen != null) {
                _selectedScreen.SetScreenActive(true);
            }
        }
    }
}