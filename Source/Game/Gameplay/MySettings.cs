using System;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Custom game settings.
    /// </summary>
    public class MySettings
    {
        /// <summary>
        /// The main menu level asset.
        /// </summary>
        public SceneReference MainMenuLevel;

        /// <summary>
        /// The main game level.
        /// </summary>
        public SceneReference GameLevel;

        /// <summary>
        /// Prefab with UI for the pause menu.
        /// </summary>
        public Prefab PauseMenuPrefab;

        /// <summary>
        /// Gets the instance of the settings from Game Settings asset.
        /// </summary>
        public static MySettings Instance
        {
            get
            {
                var settings = Engine.GetCustomSettings(nameof(MySettings));
                return settings?.Instance as MySettings;
            }
        }
    }
}
