using Sandbox.Utility;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Swift;


public class Player : Component
{
	public PlayerMovementState playerMovementState { get; private set; } = PlayerMovementState.None;
	public bool Alive { get; set; } = true;
	public CharacterController playerController { get; private set; }
	public BoxCollider collider { get; private set; }

	public DiskWeapon DiskWeapon { get; set; }

	public bool IsSpectator { 
		get
		{
			return playerMovementState == PlayerMovementState.Noclip;
		} 
	}

	public string Name
	{
		get
		{
			return Network.Owner.DisplayName;
		}
	}

	public bool CanUseTrigger { get; set; } = true;

	[Sync]
	public Guid LastAttacker { get; set; }

	[Property]
	private GameObject ClientHUD;

	[Property]
	private GameObject playerHead;
	[Property]
	private CameraComponent playerCamera;
	[Property]
	private GameObject playerShake;
	[Property]
	private GameObject playerBody;

	[Property]
	private float playerSpeed = 100f;
	[Property]
	private float playerNoclipSpeed = 1500f;
	[Property]
	private float playerGroundFriction = 5f;
	[Property]
	private float playerAirFriction = 0.3f;
	[Property]
	private float playerCameraRotation = 5f;
	[Property]
	private float playerCameraSpeed = 10f;

	[Property]
	private float cameraSensitivity = 0.1f;
	
	[Property]
	private float shakeRecoverySpeed = 1f;

	[Property]
	private DiskWeapon diskWeapon;

	private Vector3 wishVelocity = Vector3.Zero;

	private float shakeTrauma = 0;
	private Vector3 shakeMax;
	private Vector3 shakeAnglesMax;
	private float shakeFrequecy;
	private float sceneGravity;

	private float shakeSeed;

	private bool noclip = false;

	private bool grounded = false;

	public void Jump(Vector3 dir, float jumpForce)
	{
		playerController.Velocity = Vector3.Zero;
		playerController.Punch( dir * jumpForce );
	}

	public void Shake(float trauma, float frequency, Vector3 maxPos, Vector3 maxAngle)
	{
		shakeTrauma = trauma;
		shakeFrequecy = frequency;
		shakeMax = maxPos;
		shakeAnglesMax = maxAngle;
	}
	
	public void Spectate(bool turn)
	{
		if ( IsProxy )
			return;

		if (turn)
		{
			playerController.Velocity = Vector3.Zero;

			playerMovementState = PlayerMovementState.Noclip;

			playerController.Enabled = false;
			playerBody.Enabled = false;
		}
		else
		{
			playerMovementState = PlayerMovementState.None;

			playerController.Enabled = true;
			playerBody.Enabled = true;

			playerController.Velocity = Vector3.Zero;
		}
	}

	protected override void OnStart()
	{
		playerController = Components.Get<CharacterController>();

		sceneGravity = Scene.PhysicsWorld.Gravity.z;

		shakeSeed = Game.Random.Next();

		if ( IsProxy )
		{
			playerCamera.Destroy();
			ClientHUD.Destroy();
		} else
		{
			DiskWeapon = diskWeapon;
		}
	}

	protected override void OnUpdate()
	{
		CameraRotation();
		CameraMovement();
		Shaking();
		RotateBody();

		if ( IsProxy )
			return;

		if ( Input.Pressed( "Noclip" ) )
		{
			noclip = !noclip;

			Spectate( noclip );
		}
	}

	private void Shaking()
	{
		if ( IsProxy )
			return;

		if ( shakeTrauma == 0 )
			return;

		Vector3 shakeVelocity = new Vector3(
			shakeMax.x * (Noise.Perlin( shakeSeed, Time.Now * shakeFrequecy ) * 2 - 1),
			shakeMax.y * (Noise.Perlin( shakeSeed + 1, Time.Now * shakeFrequecy ) * 2 - 1),
			shakeMax.z * (Noise.Perlin( shakeSeed + 2, Time.Now * shakeFrequecy ) * 2 - 1)
			);

		shakeVelocity *= shakeTrauma;

		Angles shakeRotationalVelocity = new Angles(
			shakeAnglesMax.x * (Noise.Perlin( shakeSeed + 3, Time.Now * shakeFrequecy ) * 2 - 1),
			shakeAnglesMax.x * (Noise.Perlin( shakeSeed + 4, Time.Now * shakeFrequecy ) * 2 - 1),
			shakeAnglesMax.x * (Noise.Perlin( shakeSeed + 5, Time.Now * shakeFrequecy ) * 2 - 1)
			);

		shakeRotationalVelocity *= shakeTrauma;

		playerShake.LocalPosition = Vector3.Lerp( playerShake.LocalPosition, shakeVelocity, Time.Delta * playerCameraSpeed );
		playerShake.LocalRotation = Rotation.Lerp( playerShake.LocalRotation, shakeRotationalVelocity.ToRotation(), Time.Delta * playerCameraSpeed );

		shakeTrauma = Math.Clamp( shakeTrauma - shakeRecoverySpeed * Time.Delta, 0, 1 );
	}

	protected override void OnFixedUpdate()
	{
		if ( IsSpectator )
		{
			NoclipMovement();
		}
		else
		{
			Movement();
			Strafe();

			playerController.Accelerate( wishVelocity );
			playerController.Move();
		}
	}

	private void Strafe()
	{
		Vector3 currentVelocity = playerController.Velocity;

		float wishSpeed = wishVelocity.Length;
		Vector3 wishDirection = wishVelocity.Normal;

		if ( wishSpeed > 32 )
		{
			wishSpeed = 32;
		}

		float currentSpeed = Vector3.Dot( currentVelocity, wishDirection );
		float addSpeed = wishSpeed - currentSpeed;
		if ( addSpeed <= 0 )
			return;

		float accelSpeed = playerSpeed * 10 * Time.Delta;

		if ( accelSpeed > addSpeed )
			accelSpeed = addSpeed;

		currentVelocity.x += accelSpeed * wishDirection.x;
		currentVelocity.y += accelSpeed * wishDirection.y;

		playerController.Velocity = currentVelocity;
	}

	private Vector3 BuildDirection()
	{
		float horizontal = (Input.Down( "Right" ) ? 1 : 0) - (Input.Down( "Left" ) ? 1 : 0);
		float vertical = (Input.Down( "Forward" ) ? 1 : 0) - (Input.Down( "Backward" ) ? 1 : 0);
		float upDown = (Input.Down( "Jump" ) ? 1 : 0) - (Input.Down( "Duck" ) ? 1 : 0);

		Rotation playerRotation = playerHead.WorldRotation;
		Angles playerCameraAngles = playerCamera.LocalRotation.Angles();

		Vector3 dir = Vector3.Zero;

		dir += playerRotation.Forward * vertical;
		dir += playerRotation.Right * horizontal;

		if ( IsSpectator )
		{
			dir += Vector3.Up * upDown;
			dir -= Vector3.Up * vertical * (playerCameraAngles.pitch / 90f);
			dir -= playerRotation.Forward * vertical * Math.Abs( playerCameraAngles.pitch / 90f );
		}

		return dir.Normal;
	}

	private void Movement()
	{
		if ( IsProxy )
			return;

		wishVelocity = BuildDirection();

		if ( IsSpectator )
		{
			wishVelocity *= playerNoclipSpeed;
		}
		else
		{
			wishVelocity *= playerSpeed;
			wishVelocity = wishVelocity.WithZ( 0 );
		}

		if ( playerController.IsOnGround && !IsSpectator )
		{
			if ( !grounded )
			{
				LastAttacker = Guid.Empty;

				grounded = true;
			}
				

			playerController.ApplyFriction( playerGroundFriction );
			playerController.Velocity = playerController.Velocity.WithZ( 0 );
		}
		else
		{
			grounded = false;

			if ( !IsSpectator )
			{
				playerController.ApplyFriction( playerAirFriction );
				playerController.Velocity += Vector3.Up * sceneGravity * Time.Delta;
			}
			else
			{
				playerController.ApplyFriction( playerGroundFriction );
			}

			wishVelocity *= playerAirFriction;
		}
	}

	private void NoclipMovement()
	{
		if ( IsProxy )
			return;

		Vector3 velocity = BuildDirection();
		velocity *= playerNoclipSpeed;

		WorldPosition += velocity * Time.Delta;
	}

	private void CameraRotation()
	{
		if ( IsProxy )
			return;

		Vector2 mouseDelta = Input.MouseDelta;

		Angles headAngles = playerHead.LocalRotation.Angles();
		Angles cameraAngles = playerCamera.LocalRotation.Angles();

		headAngles.yaw -= mouseDelta.x * cameraSensitivity;

		float pitchAngle = cameraAngles.pitch + mouseDelta.y * cameraSensitivity;
		cameraAngles.pitch = Math.Clamp( pitchAngle, -89f, 89f );

		playerHead.LocalRotation = headAngles.ToRotation();
		playerCamera.LocalRotation = cameraAngles.ToRotation();
	}

	private void CameraMovement()
	{
		if ( IsProxy )
			return;

		float dir = (Input.Down( "Right" ) ? 1 : 0) - (Input.Down( "Left" ) ? 1 : 0);

		Angles playerAngles = playerCamera.LocalRotation.Angles();

		playerAngles.roll = dir * playerCameraRotation;
		Rotation playerRotation = playerAngles.ToRotation();

		Rotation defaultRotation = new Angles( playerAngles.pitch, 0, 0 );

		playerCamera.LocalRotation = Rotation.Lerp( playerCamera.LocalRotation, ((playerController.IsOnGround && playerController.Enabled) || !IsSpectator) ? playerRotation : defaultRotation, Time.Delta * playerCameraSpeed );
	}

	private void RotateBody()
	{
		playerBody.LocalRotation = playerHead.LocalRotation;
	}
}
