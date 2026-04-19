using ArizonaFramework;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Networking;

namespace Game
{
    /// <summary>
    /// Basic game menu script.
    /// </summary>
    public class PauseMenu : Script
    {
        public UIControl ContinueButton;
        public UIControl DisconnectButton;
        private int _skipFrames;

        /// <inheritdoc/>
        public override void OnStart()
        {
            if (ContinueButton)
            {
                ContinueButton.Get<Button>().Clicked += Continue;
                Engine.FocusGameViewport();
                ContinueButton.Get<Button>().Focus();
            }

            if (DisconnectButton)
            {
                DisconnectButton.Get<Button>().Clicked += Disconnect;
            }

            // Pause
            UserManager.Instance.IsGamePaused = true;
            Screen.CursorLock = CursorLockMode.None;
            Screen.CursorVisible = true;
            _skipFrames = 4;
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            if (_skipFrames-- > 0)
                return;
            if (!Engine.HasGameViewportFocus)
                return;
            if (Input.GetKeyUp(KeyboardKeys.Escape) || Input.GetGamepadButtonUp(InputGamepadIndex.All, GamepadButton.Back))
            {
                Continue();
            }
        }

        public override void OnDisable()
        {
            // Resume
            if (UserManager.Instance != null)
                UserManager.Instance.IsGamePaused = false;
            Engine.FocusGameViewport();

            base.OnDisable();
        }

        public void Continue()
        {
            Destroy(Actor);
        }

        public void Disconnect()
        {
            Destroy(Actor);
            var isLocal = !NetworkManager.IsConnected;
            GameInstance.Instance.EndGame();
            if (isLocal)
                Engine.RequestExit();
        }
    }
}
