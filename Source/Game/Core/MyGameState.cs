using ArizonaFramework;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Game State object. Managed by server, replicated to all connected clients (only server can modify it).
    /// </summary>
    public class MyGameState : GameState
    {
        /// <summary>
        /// Server game time (in seconds).
        /// </summary>
        [NetworkReplicated]
        public float MatchTime;

        /// <summary>
        /// Gets the instance of the game state.
        /// </summary>
        public static MyGameState Instance => (MyGameState)GameInstance.Instance.GameState;
    }
}
