public class DeathTeleport : Component, Component.ITriggerListener
{
	[Property]
	private GameObject teleport;

	void ITriggerListener.OnTriggerEnter(Collider other)
	{
		if ( !other.Tags.Has( "player" ) )
			return;

		other.WorldPosition = teleport.WorldPosition;

		Player playerMovement = other.Components.Get<Player>();
		if ( playerMovement == null )
			return;

		playerMovement.playerController.Velocity *= Vector3.Up;
	}
}
