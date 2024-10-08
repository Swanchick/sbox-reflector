using Sandbox.Utility;
using System;


public class Player : Component
{
	public PlayerMovementState playerMovementState { get; private set; } = PlayerMovementState.None;
	public CharacterController playerController { get; private set; }

	public DiskWeapon DiskWeapon { get; set; }

	[Property]
	private GameObject playerHead;
	[Property]
	private CameraComponent playerCamera;
	[Property]
	private GameObject playerShake;

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

	private float shakeTrauma = 0;
	private Vector3 shakeMax;
	private Vector3 shakeAnglesMax;
	private float shakeFrequecy;

	private float sceneGravity;

	private float shakeSeed;


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

	protected override void OnStart()
	{
		playerController = Components.Get<CharacterController>();

		sceneGravity = Scene.PhysicsWorld.Gravity.z;

		shakeSeed = Game.Random.Next();

		if ( !IsProxy )
		{
			DiskWeapon = diskWeapon;
			
			return;
		}
		
		playerCamera.Destroy();
	}

	protected override void OnUpdate()
	{
		CameraRotation();
		CameraMovement();
		Shaking();
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
		Movement();
	}

	private bool IsNoclip()
	{
		return playerMovementState == PlayerMovementState.Noclip;
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

		if ( IsNoclip() )
		{
			dir += Vector3.Up * upDown;
			dir -= Vector3.Up * vertical * (playerCameraAngles.pitch / 90f);
			dir -= playerRotation.Forward * vertical * Math.Abs( playerCameraAngles.pitch / 90f );
		}

		return dir.Normal;
	}

	private void CameraMovement()
	{
		if ( IsProxy )
			return;

		float dir = (Input.Down("Right") ? 1 : 0) - (Input.Down("Left") ? 1 : 0);

		Angles playerAngles = playerCamera.LocalRotation.Angles();

		playerAngles.roll = dir * playerCameraRotation;
		Rotation playerRotation = playerAngles.ToRotation();

		Rotation defaultRotation = new Angles( playerAngles.pitch, 0, 0 );

		playerCamera.LocalRotation = Rotation.Lerp( playerCamera.LocalRotation, (playerController.IsOnGround) ? playerRotation : defaultRotation, Time.Delta * playerCameraSpeed );
	}

	private void Noclip()
	{
		switch (playerMovementState)
		{
			case PlayerMovementState.None:
				playerMovementState = PlayerMovementState.Noclip;
				break;
			case PlayerMovementState.Noclip:
				playerMovementState = PlayerMovementState.None;
				break;
		}
	}

	private void Movement()
	{
		if ( IsProxy )
			return;

		Vector3 velocity = BuildDirection();

		if ( IsNoclip() )
		{
			velocity *= playerNoclipSpeed;
		}
		else
		{
			velocity *= playerSpeed;
			velocity = velocity.WithZ( 0 );
		}
			
		
		if (Input.Pressed("Noclip"))
		{
			Noclip();
		}

		if ( playerController.IsOnGround && !IsNoclip() )
		{
			playerController.ApplyFriction( playerGroundFriction );
			playerController.Velocity = playerController.Velocity.WithZ( 0 );
		}
		else
		{
			if ( !IsNoclip() )
			{
				playerController.ApplyFriction( playerAirFriction );
				playerController.Velocity += Vector3.Up * sceneGravity * Time.Delta;
			}
			else
			{
				playerController.ApplyFriction( playerGroundFriction );
			}

			velocity *= playerAirFriction;
		}

		playerController.Accelerate( velocity );
		playerController.Move();
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
}
