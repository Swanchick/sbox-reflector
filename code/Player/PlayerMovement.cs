using System;

public sealed class PlayerMovement : Component 
{
	[Property]
	public Player Player { get; set; }

	public enum State 
	{
		None,
		Grounded,
		Noclip,
	}

	private State currentState = State.None;

	public bool IsSpectator 
	{
		get 
		{
			return currentState == State.Noclip;
		}
	}

	public bool IsGrounded 
	{
		get
		{
			return currentState == State.Grounded;
		}
	}

	public bool CanUseTrigger { get; set; } = true;

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

	public CharacterController PlayerController;

	private Vector3 wishVelocity = Vector3.Zero;

	private float sceneGravity;

	public void Jump(Vector3 dir, float jumpForce)
	{
		PlayerController.Velocity = Vector3.Zero;
		PlayerController.Punch( dir * jumpForce );
	}

	public void Jump( float jumpForce )
	{
		PlayerController.Punch( Vector3.Up * jumpForce );
	}

	protected override void OnStart()
	{
		PlayerController = Components.Get<CharacterController>();
		
		
		sceneGravity = Scene.PhysicsWorld.Gravity.z;
	}

	protected override void OnFixedUpdate()
	{
		switch (currentState) 
		{
			case State.Noclip:
				NoclipMovement();
				break;
			default:
				Movement();
				Strafe();
				GetJump();

				PlayerController.Accelerate( wishVelocity );
				PlayerController.Move();

				break;
		}
	}

	private void Strafe()
	{
		Vector3 currentVelocity = PlayerController.Velocity;

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

		PlayerController.Velocity = currentVelocity;
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

		if ( PlayerController.IsOnGround && !IsSpectator )
		{
			if ( currentState != State.Grounded )
			{
				Player.LastAttacker = Guid.Empty;

				currentState = State.Grounded;
			}
				

			PlayerController.ApplyFriction( playerGroundFriction );
			PlayerController.Velocity = PlayerController.Velocity.WithZ( 0 );
		}
		else
		{
			currentState = State.None;

			PlayerController.ApplyFriction( playerAirFriction );
			PlayerController.Velocity += Vector3.Up * sceneGravity * Time.Delta;

			wishVelocity *= playerAirFriction;
		}
	}

	private void GetJump()
	{
		if ( IsProxy )
			return;

		if ( !Input.Pressed( "Jump" ) )
			return;

		if ( !IsGrounded )
			return;

		Jump( 250f );
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
