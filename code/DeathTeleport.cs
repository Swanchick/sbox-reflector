public class DeathTeleport : BaseTrigger
{
	[Property]
	private GameObject teleport;

	protected override void OnPlayerEnter( Player player )
	{
		player.WorldPosition = teleport.WorldPosition;
		player.playerController.Velocity *= Vector3.Up;
	}
}
