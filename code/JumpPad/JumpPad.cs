using Sandbox;

public class JumpPad : BaseTrigger
{
	[Property]
	private Vector3 jumpDirection;

	[Property]
	private float jumpForce = 500f;

	protected override void OnPlayerEnter( Player player )
	{
		if ( !player.Alive )
			return;

		player.Movement.Jump( jumpDirection, jumpForce );
		player.CameraMovement.Shake( 10f, 100, new Vector3( 3, 3, 3 ), new Vector3( 4, 4, 4 ) );
	}
}
