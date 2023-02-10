using System;
using ArizonaFramework;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Game Mode script. Exists only on server.
    /// </summary>
    public class MyGameMode : GameMode
    {
        /// <summary>
        /// Gets the instance of the game mode. Server/host only, null on clients.
        /// </summary>
        public static MyGameMode Instance => (MyGameMode)GameInstance.Instance.GameMode;

        /// <inheritdoc/>
        public override void StartGame()
        {
            base.StartGame();

            Debug.Log("[Arizona] MyGameMode.StartGame");

            Scripting.Update += OnUpdate;
        }

        /// <inheritdoc/>
        public override void StopGame()
        {
            Debug.Log("[Arizona] MyGameMode.StopGame");

            Scripting.Update -= OnUpdate;

            base.StopGame();
        }

        /// <inheritdoc/>
        public override void OnPlayerSpawned(PlayerState playerState)
        {
            base.OnPlayerSpawned(playerState);

            RespawmPlayer(playerState);
        }

        /// <summary>
        /// Spawns player at random spawn point and resets health.
        /// </summary>
        /// <param name="playerState">The player to respawm.</param>
        public void RespawmPlayer(PlayerState playerState)
        {
            // Reset health
            var state = (MyPlayerState)playerState;
            state.Health = 100.0f;

            // Spawn player at proper location
            var spawnPoints = LevelsManager.Instance.SpawnPoints;
            if (spawnPoints.Count != 0)
            {
                var spawnPoint = spawnPoints[RandomUtil.Random.Next(spawnPoints.Count)];
                playerState.PlayerPawn.Transform = new Transform(spawnPoint.Position, spawnPoint.Orientation);
                Debug.Log($"[Arizona] Spawning player {playerState.PlayerId} at {spawnPoint.GetNamePath()}");
            }
        }

        private void OnUpdate()
        {
            var gameState = MyGameState.Instance;
            if (gameState == null)
                throw new Exception("Missing game state.");

            gameState.MatchTime += Time.DeltaTime;
        }
    }
}
