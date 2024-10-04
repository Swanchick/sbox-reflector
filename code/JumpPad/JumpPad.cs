using Sandbox;

public class JumpPad : Component, Component.ITriggerListener
{
	[Property]
	private Vector3 jumpDirection;

	[Property]
	private float jumpForce = 500f;
	
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( !other.Tags.Has( "player" ) )
			return;

		PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

		if ( playerMovement == null )
			return;

		playerMovement.Jump( jumpDirection, jumpForce );
	}
}
