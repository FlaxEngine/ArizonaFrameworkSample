using ArizonaFramework;
using FlaxEngine;
using FlaxEngine.Networking;

namespace Game
{
    /// <summary>
    /// Player Pawn script. Represents player object on a scene.
    /// </summary>
    public class MyPlayerPawn : PlayerPawn, IGameEntity
    {
        public StaticModel VisualMesh;
        public Actor CameraTarget;
        public Camera Camera;
        public CharacterController Controller => (CharacterController)Actor;

        [EditorDisplay("Shooting")]
        public ParticleEmitter ShootRibbonTrailParticles;

        [EditorDisplay("Shooting")]
        public float Damage = 30.0f;

        /// <inheritdoc />
        public override void OnPlayerSpawned()
        {
            base.OnPlayerSpawned();

            Debug.Log("[Arizona] MyPlayerPawn.OnPlayerSpawned PlayerId=" + PlayerId);

            // Set mesh color to match the player id
            var material = VisualMesh.CreateAndSetVirtualMaterialInstance(0);
            var colors = new[]
            {
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.Yellow,
                Color.Purple,
                Color.Orange,
            };
            material.SetParameterValue("Color", colors[PlayerId % colors.Length]);

            if (PlayerState.NetworkClientId == NetworkManager.LocalClientId)
            {
                if (NetworkManager.IsClient)
                {
                    // Allow local client to simulate this pawn for smoother gameplay (pawn still gets validated again server simulation to reduce cheats)
                    var ownerClientId = NetworkReplicator.GetObjectOwnerClientId(this);
                    NetworkReplicator.SetObjectOwnership(Actor, ownerClientId, NetworkObjectRole.ReplicatedSimulated, true);
                }

                // Hide player body locally (for First Person Camera)
                VisualMesh.LayerName = "Local Player";

                // Cut rendering of temporal effects
                MainRenderTask.Instance.CameraCut();
            }
            else
            {
                // Disable remote player camera
                Camera.IsActive = false;
            }
        }

        /// <summary>
        /// Draws bullet trajectory on all clients.
        /// </summary>
        /// <param name="start">World-space bullet trajectory start.</param>
        /// <param name="end">World-space bullet trajectory start.</param>
        [NetworkRpc(Client = true)]
        public void OnShootRpc(Vector3 start, Vector3 end)
        {
            // Spawn gun trail
            // TODO: use pooling
            // TODO: skip visuals on server
            var trailSystem = Content.CreateVirtualAsset<ParticleSystem>();
            trailSystem.Init(ShootRibbonTrailParticles, 1.0f, 30.0f);
            var trailEffect = trailSystem.Spawn(Vector3.Zero, true);
            trailEffect.SetParameterValue(null, "StartPos", start - new Float3(0, 20, 0));
            trailEffect.SetParameterValue(null, "EndPos", end);

            // TODO: spawn decal at hit end
            // TODO: play SFX
        }

        /// <inheritdoc />
        public bool CanDamage => true;

        /// <inheritdoc />
        public void ApplyDamage(float amount, Vector3 location, Vector3 direction, IGameEntity causer)
        {
            var playerState = (MyPlayerState)PlayerState;
            playerState.Health -= amount;
            if (playerState.Health < 0)
            {
                playerState.Health = 0;
                Debug.Log("[Arizona] PlayerId=" + PlayerId + " killed");
                // TODO: respawm counter
                MyGameMode.Instance.RespawmPlayer(playerState);
            }
        }
    }
}
