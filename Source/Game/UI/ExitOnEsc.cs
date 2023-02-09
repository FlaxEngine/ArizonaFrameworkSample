using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Utility script to exit game with Escape key.
    /// </summary>
    public class ExitOnEsc : Script
    {
        /// <inheritdoc />
        public override void OnUpdate()
        {
            if (!Engine.HasGameViewportFocus)
                return;
            if (Input.GetKeyUp(KeyboardKeys.Escape) || Input.GetGamepadButtonUp(InputGamepadIndex.All, GamepadButton.Back))
            {
                var mainMenu = Level.FindScript<MainMenu>();
                var pauseMenu = Level.FindScript<PauseMenu>();
                if (mainMenu)
                    Engine.RequestExit();
                else if (pauseMenu)
                {
                }
                else
                {
                    // Show pause menu
                    var mySettings = MySettings.Instance;
                    PrefabManager.SpawnPrefab(mySettings.PauseMenuPrefab);
                }
            }
        }
    }
}
