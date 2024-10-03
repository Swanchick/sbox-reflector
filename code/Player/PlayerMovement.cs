using Sandbox;
using System;


public class PlayerMovement : Component
{
	[Property]
	private GameObject playerHead;
	[Property]
	private CameraComponent playerCamera;
	//[Property]
	//private GameObject playerBody;

	[Property]
	private float playerSpeed = 100f;
	[Property]
	private float playerGroundFriction = 5f;
	[Property]
	private float playerAirFriction = 0.3f;
	[Property]
	private float playerJumpForce = 100f;

	[Property]
	private float cameraSensitivity = 0.1f;


	private float sceneGravity;

	private CharacterController playerController;

	protected override void OnStart()
	{
		playerController = Components.Get<CharacterController>();

		sceneGravity = Scene.PhysicsWorld.Gravity.z;
	}

	protected override void OnUpdate()
	{
		CameraRotation();
	}

	protected override void OnFixedUpdate()
	{
		Movement();
	}

	private Vector3 BuildDirection()
	{
		float horizontal = (Input.Down( "Right" ) ? 1 : 0) - (Input.Down( "Left" ) ? 1 : 0);
		float vertical = (Input.Down( "Forward" ) ? 1 : 0) - (Input.Down( "Backward" ) ? 1 : 0);

		Rotation playerRotation = playerHead.WorldRotation;
		Vector3 dir = playerRotation.Forward * vertical + playerRotation.Right * horizontal;

		return dir.Normal;
	}

	private void Movement()
	{
		if ( IsProxy )
			return;

		Vector3 velocity = BuildDirection();
		velocity = velocity.WithZ( 0 );
		velocity *= playerSpeed;

		if ( playerController.IsOnGround )
		{
			playerController.ApplyFriction( playerGroundFriction );
			playerController.Velocity = playerController.Velocity.WithZ( 0 );

			if ( Input.Down( "Jump" ) )
			{
				playerController.Punch( Vector3.Up * playerJumpForce );
			}
		}
		else
		{
			playerController.ApplyFriction( playerAirFriction );
			playerController.Velocity += Vector3.Up * sceneGravity * Time.Delta;

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
