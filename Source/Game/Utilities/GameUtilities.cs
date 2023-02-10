using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Various utility methods for game.
    /// </summary>
    public static class GameUtilities
    {
        /// <summary>
        /// Gets entity from a given actor.
        /// </summary>
        /// <param name="actor">Target actor.</param>
        /// <returns>Found entity or null if nothing found.</returns>
        public static IGameEntity GetEntity(this Actor actor)
        {
            if (actor == null)
                return null;
            var entity = actor as IGameEntity;
            if (entity == null)
            {
                entity = actor.GetScript<IGameEntity>();
                if (entity == null && actor.HasParent)
                    entity = actor.Parent.GetEntity();
            }
            return entity;
        }
    }
}
