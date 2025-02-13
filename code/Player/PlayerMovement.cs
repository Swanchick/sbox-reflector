using System;

public sealed class PlayerMovement : Component 
{
	[Property]
	public Player Player { get; set; }

	public enum State 
	{
		None,
		Grounded,
		Spectator,
	}

	private State currentState = State.None;

	public bool IsSpectator 
	{
		get 
		{
			return currentState == State.Spectator;
		}
	}

	public bool IsGrounded 
	{
		get
		{
			return currentState == State.Grounded;
		}
	}

	[Property]
	private float playerSpeed = 100f;
	[Property]
	private float playerNoclipSpeed = 1500f;
	[Property]
	private float playerGroundFriction = 5f;
	[Property]
	private float playerAirFriction = 0.3f;

	[Property]
	private GameObject playerHead;
	[Property]
	private GameObject playerCamera;

	private CharacterController playerController;

	private Vector3 wishVelocity = Vector3.Zero;

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
	}

	protected override void OnFixedUpdate()
	{
		switch (currentState) 
		{
			case State.Spectator:
				NoclipMovement();
				break;
			default:
				Movement();
				Strafe();


				playerController.Accelerate( wishVelocity );
				playerController.Move();

				break;
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
			if ( currentState != State.Grounded )
			{
				// LastAttacker = Guid.Empty;

				currentState = State.Grounded;
			}
				

			playerController.ApplyFriction( playerGroundFriction );
			playerController.Velocity = playerController.Velocity.WithZ( 0 );
		}
		else
		{
			currentState = State.None;

			playerController.ApplyFriction( playerAirFriction );
			playerController.Velocity += Vector3.Up * sceneGravity * Time.Delta;

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
}
