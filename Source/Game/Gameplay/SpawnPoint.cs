using FlaxEngine;
#if FLAX_EDITOR
using System.IO;
using FlaxEditor;
using FlaxEditor.SceneGraph;
#endif

namespace Game
{
    /// <summary>
    /// Spawn Point actor placed on a level to mark players spawn location.
    /// </summary>
    public class SpawnPoint : Actor
    {
#if FLAX_EDITOR
        static SpawnPoint()
        {
            ViewportIconsRenderer.AddCustomIcon(typeof(SpawnPoint), Content.LoadAsync<Texture>(Path.Combine(Globals.ProjectContentFolder, "Textures/spawn-point-icon.flax")));
            SceneGraphFactory.CustomNodesTypes.Add(typeof(SpawnPoint), typeof(SpawnPointNode));
        }
#endif

        /// <inheritdoc />
        public override void OnEnable()
        {
            base.OnEnable();
#if FLAX_EDITOR
            ViewportIconsRenderer.AddActor(this);
#endif
            LevelsManager.Instance?.SpawnPoints.Add(this);
        }

        /// <inheritdoc />
        public override void OnDisable()
        {
            LevelsManager.Instance?.SpawnPoints.Remove(this);
#if FLAX_EDITOR
            ViewportIconsRenderer.RemoveActor(this);
#endif
            base.OnDisable();
        }
    }

#if FLAX_EDITOR
    /// <summary>Custom actor node for Editor.</summary>
    [HideInEditor]
    public sealed class SpawnPointNode : ActorNodeWithIcon
    {
        /// <inheritdoc />
        public SpawnPointNode(Actor actor)
            : base(actor)
        {
        }
    }
#endif
}
