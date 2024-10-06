using Sandbox;
using System;


public class PlayerMovement : Component
{
	public PlayerMovementState playerMovementState { get; private set; } = PlayerMovementState.None;
	public CharacterController playerController { get; private set; }

	[Property]
	private GameObject playerHead;
	[Property]
	private CameraComponent playerCamera;

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
	private float cameraSensitivity = 0.1f;

	private float sceneGravity;


	public void Jump(Vector3 dir, float jumpForce)
	{
		playerController.Velocity = Vector3.Zero;
		playerController.Punch( dir * jumpForce );
	}

	protected override void OnStart()
	{
		playerController = Components.Get<CharacterController>();

		sceneGravity = Scene.PhysicsWorld.Gravity.z;

		if ( IsProxy )
		{
			playerCamera.Destroy();

			return;
		}
	}

	protected override void OnUpdate()
	{
		CameraRotation();
		CameraMovement();
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

		playerCamera.LocalRotation = Rotation.Lerp( playerCamera.LocalRotation, (playerController.IsOnGround) ? playerRotation : defaultRotation, 10f * Time.Delta );
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
