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

		Player player = other.GetComponent<Player>();

		if ( player == null )
			return;

		player.Jump( jumpDirection, jumpForce );
		player.Shake( 10f, 100, new Vector3( 3, 3, 3 ), new Vector3( 4, 4, 4 ) );
	}
}
