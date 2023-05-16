using System;
using ArizonaFramework;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Player Controller script. Spawned on server and local client. Allows local client to control its player (gets validated by server).
    /// </summary>
    public class MyPlayerController : PlayerController
    {
        [EditorDisplay("Movement")]
        public float Friction = 8.0f;

        [EditorDisplay("Movement")]
        public float GroundAccelerate = 5000;

        [EditorDisplay("Movement")]
        public float AirAccelerate = 10000;

        [EditorDisplay("Movement")]
        public float MaxVelocityGround = 400;

        [EditorDisplay("Movement")]
        public float MaxVelocityAir = 200;

        [EditorDisplay("Input")]
        public bool UseMouse = true;

        [EditorDisplay("Input")]
        public bool CanJump = true;

        [EditorDisplay("Input")]
        public bool CanShoot = true;

        [EditorDisplay("Input")]
        public float JumpForce = 800;

        [EditorDisplay("Camera")]
        public float CameraSmoothing = 20.0f;

        private Vector3 _velocity;
        private float _pitch;
        private float _yaw;
        private float _horizontal;
        private float _vertical;

        /// <summary>
        /// Current player camera view direction (in world-space). Managed by client and replicated to server.
        /// </summary>
        [ReadOnly, NetworkReplicated, EditorDisplay("Debug")]
        public Quaternion ViewDirection;

        /// <summary>
        /// Adds the movement and rotation to the camera (as input).
        /// </summary>
        /// <param name="horizontal">The horizontal input.</param>
        /// <param name="vertical">The vertical input.</param>
        /// <param name="pitch">The pitch rotation input.</param>
        /// <param name="yaw">The yaw rotation input.</param>
        public void AddMovementRotation(float horizontal, float vertical, float pitch, float yaw)
        {
            _pitch += pitch;
            _yaw += yaw;
            _horizontal += horizontal;
            _vertical += vertical;
        }

        /// <inheritdoc />
        public override void OnPlayerSpawned()
        {
            base.OnPlayerSpawned();

            Debug.Log("[Arizona] MyPlayerController.OnPlayerSpawned PlayerId=" + PlayerId);
        }

        /// <inheritdoc />
        public override void OnDisable()
        {
            base.OnDisable();

            if (UseMouse)
            {
                Screen.CursorVisible = true;
                Screen.CursorLock = CursorLockMode.None;
            }
        }

        /// <inheritdoc />
        public override void OnUpdateInput()
        {
            var pawn = PlayerPawn as MyPlayerPawn;
            if (pawn == null)
                throw new Exception("Missing player pawn.");
            var useInput = Engine.HasGameViewportFocus && !UserManager.Instance.IsGamePaused;

            // Movement
            if (useInput)
            {
                _horizontal += Input.GetAxis("Horizontal");
                _vertical += Input.GetAxis("Vertical");
            }

            if (useInput && UseMouse)
            {
                // Cursor
                Screen.CursorVisible = false;
                Screen.CursorLock = CursorLockMode.Locked;

                // Mouse
                var mouseDelta = new Float2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                _pitch = Mathf.Clamp(_pitch + mouseDelta.Y, -88, 88);
                _yaw += mouseDelta.X;
            }

            // Jump
            bool jump = useInput && CanJump && Input.GetAction("Jump");

            // Shoot
            if (useInput && CanShoot && Input.GetAction("Shoot"))
            {
                // TODO: for smoother gameplay local client could run shooting visualization before server confirms result
                var shootRay = GetCameraRay();
                ShootRpc(shootRay);
            }

            // Update camera
            var camTrans = pawn.Camera.Transform;
            var camFactor = Mathf.Saturate(CameraSmoothing * Time.DeltaTime);
            pawn.CameraTarget.LocalOrientation = Quaternion.Lerp(pawn.CameraTarget.LocalOrientation, Quaternion.Euler(_pitch, _yaw, 0), camFactor);
            camTrans.Translation = Vector3.Lerp(camTrans.Translation, pawn.CameraTarget.Position, camFactor);
            camTrans.Orientation = pawn.CameraTarget.Orientation;
            pawn.Camera.Transform = camTrans;

            // Calculate player movement vector
            var velocity = new Vector3(_horizontal, 0.0f, _vertical);
            _horizontal = 0;
            _vertical = 0;
            velocity.Normalize();
            Vector3 rotation = pawn.Camera.EulerAngles;
            rotation.X = 0;
            rotation.Z = 0;
            velocity = Vector3.Transform(velocity, Quaternion.Euler(rotation));
            if (pawn.Controller.IsGrounded)
            {
                velocity = MoveGround(velocity.Normalized, Horizontal(_velocity));
                velocity.Y = -Mathf.Abs(Physics.Gravity.Y * 0.5f);
            }
            else
            {
                velocity = MoveAir(velocity.Normalized, Horizontal(_velocity));
                velocity.Y = _velocity.Y;
            }

            // Fix direction
            if (velocity.Length < 0.05f)
                velocity = Vector3.Zero;

            // Jump
            if (jump && pawn.Controller.IsGrounded)
                velocity.Y = JumpForce;

            // Apply gravity
            velocity.Y += -Mathf.Abs(Physics.Gravity.Y * 2.5f) * Time.DeltaTime;

            // Check if player is not blocked by something above head
            if ((pawn.Controller.Flags & CharacterController.CollisionFlags.Above) != 0)
            {
                if (velocity.Y > 0)
                {
                    // Player head hit something above, zero the gravity acceleration
                    velocity.Y = 0;
                }
            }

            // Move
            MovePawn(velocity * Time.DeltaTime, Quaternion.Identity);
            _velocity = velocity;
            ViewDirection = pawn.Camera.Orientation;
        }

        /// <inheritdoc />
        public override bool OnValidateMove(Vector3 translation, Quaternion rotation)
        {
            // TODO: validate movement to prevent player cheating (server-authority)
            return base.OnValidateMove(translation, rotation);
        }

        /// <summary>
        /// Server RPC called when local player wants to shoot. Executed on a server to validate the state and apply damage on server to hit enemy.
        /// </summary>
        /// <param name="shootRay">World-space ray of the bullet trajectory.</param>
        [NetworkRpc(Server = true)]
        private void ShootRpc(Ray shootRay)
        {
            var pawn = (MyPlayerPawn)PlayerPawn;
            // TODO: validate input ray position/direction to prevent player cheating (server-authority)

            // Execute shooting
            // TODO: implement weapon spread (based on player velocity)
            var maxDistance = 10000.0f;
            shootRay.Position += shootRay.Direction * 42.0f;
            var hit = Physics.RayCast(shootRay.Position, shootRay.Direction, out var hitInfo, maxDistance, uint.MaxValue, false);
            var hitEnd = shootRay.GetPoint(hit ? hitInfo.Distance : maxDistance);
            /*if (hit)
            {
                DebugDraw.DrawSphere(new BoundingSphere(shootRay.Position, 5.0f), Color.Gray, 100000, true);
                DebugDraw.DrawSphere(new BoundingSphere(hitEnd, 5.0f), Color.Black, 100000, true);
                DebugDraw.DrawLine(shootRay.Position, hitEnd, Color.Red, 10000, true);
            }*/

            // Apply damage
            if (hit)
            {
                var entity = hitInfo.Collider.GetEntity();
                if (entity != null && entity != (object)pawn && entity.CanDamage)
                    entity.ApplyDamage(pawn.Damage, hitEnd, shootRay.Direction, pawn);
            }

            // Render visuals (on all connected clients via Client RPC)
            pawn.OnShootRpc(shootRay.Position, hitEnd);
        }

        private Ray GetCameraRay()
        {
            var pawn = (MyPlayerPawn)PlayerPawn;
            return new Ray(pawn.Camera.Position, Float3.Forward * ViewDirection);
        }

        // accelDir: normalized direction that the player has requested to move (taking into account the movement keys and look direction)
        // prevVelocity: The current velocity of the player, before any additional calculations
        // accelerate: The server-defined player acceleration value
        // maxVelocity: The server-defined maximum player velocity (this is not strictly adhered to due to strafejumping)
        private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxVelocity)
        {
            float projVel = (float)Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir
            float accelVel = accelerate * Time.DeltaTime; // Accelerated velocity in direction of movement

            // If necessary, truncate the accelerated velocity so the vector projection does not exceed max velocity
            if (projVel + accelVel > maxVelocity)
                accelVel = maxVelocity - projVel;

            return prevVelocity + accelDir * accelVel;
        }

        private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
        {
            // Apply Friction
            var speed = prevVelocity.Length;
            if (Math.Abs(speed) > 0.01f) // To avoid divide by zero errors
            {
                var drop = speed * Friction * Time.DeltaTime;
                prevVelocity *= Mathf.Max(speed - drop, 0) / speed; // Scale the velocity based on friction
            }

            // GroundAccelerate and MaxVelocityGround are server-defined movement variables
            return Accelerate(accelDir, prevVelocity, GroundAccelerate, MaxVelocityGround);
        }

        private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
        {
            // air_accelerate and max_velocity_air are server-defined movement variables
            return Accelerate(accelDir, prevVelocity, AirAccelerate, MaxVelocityAir);
        }

        private Vector3 Horizontal(Vector3 v)
        {
            return new Vector3(v.X, 0, v.Z);
        }
    }
}
