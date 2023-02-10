using ArizonaFramework;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game
{
    /// <summary>
    /// Player UI script. Spawned for local player to display UI and HUD.
    /// </summary>
    public class MyPlayerUI : PlayerUI
    {
        public UIControl TextLabel;

        /// <inheritdoc />
        public override void OnPlayerSpawned()
        {
            base.OnPlayerSpawned();

            Debug.Log("[Arizona] MyPlayerUI.OnPlayerSpawned PlayerId=" + PlayerState.PlayerId);
        }
        
        /// <inheritdoc />
        public override void OnUpdate()
        {
            base.OnUpdate();

            var playerState = (MyPlayerState)PlayerState;
            var gameState = MyGameState.Instance;
            if (playerState == null || gameState == null)
                return;

            var textLabel = TextLabel?.Get<Label>();
            if (textLabel != null)
            {
                textLabel.Text = $"Player UI for PlayerId: {PlayerState.PlayerId}\nMatch Time: {(int)gameState.MatchTime}s\nHealth: {playerState.Health}";
            }
        }
    }
}
