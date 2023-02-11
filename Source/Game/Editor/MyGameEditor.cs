#if FLAX_EDITOR
using System;
using ArizonaFramework;
using FlaxEditor;
using FlaxEditor.Content;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game.Editor
{
    /// <summary>
    /// Custom Editor-extensions for game implemented as Editor Plugin.
    /// </summary>
    public class MyGameEditor : EditorPlugin
    {
        private CustomSettingsProxy _mySettingsProxy;

        public MyGameEditor()
        {
            _description = new PluginDescription
            {
                Name = "MyGameEditor",
                Description = "Custom Editor-extensions for game implemented as Editor Plugin.",
                Version = new Version(1, 0),
                Category = "Game",
                Author = "Flax",
                RepositoryUrl = "https://github.com/FlaxEngine/ArizonaFrameworkSample",
            };
        }

        /// <inheritdoc />
        public override void InitializeEditor()
        {
            base.InitializeEditor();

            Editor.PlayModeBegin += OnPlayModeBegin;

            // Register game settings for easier editing
            _mySettingsProxy = new CustomSettingsProxy(typeof(MySettings), "MySettings");
            Editor.ContentDatabase.Proxy.Add(_mySettingsProxy);
        }

        /// <inheritdoc />
        public override void DeinitializeEditor()
        {
            // Cleanup
            Editor.PlayModeBegin -= OnPlayModeBegin;
            Editor.ContentDatabase.Proxy.Remove(_mySettingsProxy);

            base.DeinitializeEditor();
        }

        private void OnPlayModeBegin()
        {
            // When starting play mode in editor on game level 0not menu) start a local play
            var mySettings = MySettings.Instance;
            if (mySettings == null)
                return;
            if (NetworkManager.IsOffline && Level.FindScene(mySettings.MainMenuLevel.ID) == null)
            {
                // Play locally
                GameInstance.Instance.StartGame();
                GameInstance.Instance.SpawnLocalPlayer();
            }
        }
    }
}
#endif
