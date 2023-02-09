using ArizonaFramework;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Player State object. Server-managed and replicated to all clients information about specific player.
    /// </summary>
    public class MyPlayerState : PlayerState
    {
        /// <summary>
        /// Current player health. Changed by server, replicated to all clients.
        /// </summary>
        [NetworkReplicated]
        public float Health = 100.0f;
    }
}
