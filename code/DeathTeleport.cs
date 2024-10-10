public class DeathTeleport : BaseTrigger
{
	[Property]
	private GameObject teleport;

	private Reflector reflector;

	protected override void OnStart()
	{
		GameObject reflectorObject = Scene.Directory.FindByName( "ReflectorManager" ).FirstOrDefault();
		if ( reflector == null )
			return;

		reflector = reflectorObject.Components.Get<Reflector>();
	}

	protected override void OnPlayerEnter( Player player )
	{
		player.WorldPosition = teleport.WorldPosition;
		player.playerController.Velocity *= Vector3.Up;

		reflector.OnPlayerDeath( player.GameObject, player );
	}
}
