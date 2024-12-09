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

		Log.Info($"{GameObject.Name}/{GameObject.Parent.Name}/{GameObject.Parent.Parent.Name} - hitted");

		player.Jump( jumpDirection, jumpForce );
		player.Shake( 10f, 100, new Vector3( 3, 3, 3 ), new Vector3( 4, 4, 4 ) );
	}
}
