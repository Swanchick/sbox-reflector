public class DeathTeleport : Component, Component.ITriggerListener
{
	[Property]
	private GameObject teleport;

	void ITriggerListener.OnTriggerEnter(Collider other)
	{
		if ( !other.Tags.Has( "player" ) )
			return;

		other.WorldPosition = teleport.WorldPosition;

		PlayerMovement playerMovement = other.Components.Get<PlayerMovement>();
		if ( playerMovement == null )
			return;

		playerMovement.playerController.Velocity = Vector3.Zero;
	}
}
