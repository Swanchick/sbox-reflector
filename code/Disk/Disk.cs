using Sandbox;

public class Disk : Component, Component.ICollisionListener
{
	[Property]
	private float diskSpeed = 100f;
	[Property]
	private float collideForce = 200f;

	private CharacterController diskController;


	protected override void OnStart()
	{
		diskController = Components.Get<CharacterController>();
	}

	protected override void OnUpdate()
	{
		diskController.Velocity = WorldRotation.Forward * diskSpeed;

		diskController.Move();
	}

	void ICollisionListener.OnCollisionStart(Collision collision)
	{
		Log.Info( "Collided!!!" );
		
		var other = collision.Self;
		GameObject gameObject = other.GameObject;
		if ( gameObject == null )
			return;

		if ( !gameObject.Tags.Has( "player" ) )
			return;

		PlayerMovement playerMovement = gameObject.GetComponent<PlayerMovement>();
		if ( playerMovement == null )
			return;

		playerMovement.Jump( WorldRotation.Forward + Vector3.Up, collideForce );

		
	}
}
