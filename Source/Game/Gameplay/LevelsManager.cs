using System.Collections.Generic;
using ArizonaFramework;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game
{
    /// <summary>
    /// Game levels manager that handles scene transitions (eg. joining game map after main menu connection).
    /// </summary>
    public class LevelsManager : GameSystem
    {
        /// <summary>
        /// Gets the Levels Manager instance.
        /// </summary>
        public static LevelsManager Instance => GameInstance.Instance?.GetGameSystem<LevelsManager>();

        /// <summary>
        /// Active spawn points.
        /// </summary>
        public readonly List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            NetworkManager.StateChanged += OnNetworkStateChanged;
        }

        /// <inheritdoc />
        public override void Deinitialize()
        {
            NetworkManager.StateChanged -= OnNetworkStateChanged;

            base.Deinitialize();
        }

        private void OnNetworkStateChanged()
        {
#if FLAX_EDITOR
            // Skip level transitions when editor starts/end splay mode
            var playingState = FlaxEditor.Editor.Instance.StateMachine.PlayingState;
            if (playingState.IsPlayModeStarting || playingState.IsPlayModeEnding)
                return;
#endif

            // Select target scene to go to
            var mySettings = MySettings.Instance;
            var targetScene = mySettings.MainMenuLevel; // Go to menu by default
            if (NetworkManager.State == NetworkConnectionState.Connected)
            {
                targetScene = mySettings.GameLevel; // Go to the game level
            }

            // Load that scene (skip if already loaded)
            if (Level.FindScene(targetScene.ID) != null)
                return;
            Level.UnloadAllScenesAsync();
            Level.LoadSceneAsync(targetScene);
        }
    }
}
