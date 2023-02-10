using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Interface for game objects.
    /// </summary>
    public interface IGameEntity
    {
        /// <summary>
        /// True if entity can be damaged.
        /// </summary>
        bool CanDamage { get; }

        /// <summary>
        /// Applies damage to the entity.
        /// </summary>
        /// <param name="amount">Amount of the damage.</param>
        /// <param name="location">Hit location.</param>
        /// <param name="direction">Hit direction.</param>
        /// <param name="causer">Entity that caused damage.</param>
        void ApplyDamage(float amount, Vector3 location, Vector3 direction, IGameEntity causer);
    }
}
