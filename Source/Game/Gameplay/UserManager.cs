using ArizonaFramework;

namespace Game
{
    /// <summary>
    /// Local user manager (for gameplay).
    /// </summary>
    public class UserManager : GameSystem
    {
        /// <summary>
        /// True if game was paused (eg. via pause menu).
        /// </summary>
        public bool IsGamePaused;

        /// <summary>
        /// Gets the User Manager instance.
        /// </summary>
        public static UserManager Instance => GameInstance.Instance?.GetGameSystem<UserManager>();
    }
}
